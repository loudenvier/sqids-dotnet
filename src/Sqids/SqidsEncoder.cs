#if NET7_0_OR_GREATER
using System.Numerics;
using System.Runtime.CompilerServices;
#else
using System.Runtime.CompilerServices;
using T = System.UInt64; // Alias used to unify .NET 7+ and legacy code
#endif

namespace Sqids;

#if NET7_0_OR_GREATER
/// <summary>
/// The Sqids encoder/decoder. This is the main class.
/// </summary>
/// <typeparam name="T">
/// The integral numeric type that will be encoded/decoded.
/// Could be one of `int`, `long`, `byte`, `short`, and others. For the full list, check out
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
/// </typeparam>
public sealed class SqidsEncoder<T> where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
#else
/// <summary>
/// The Sqids encoder/decoder. This is the main class.
/// </summary>
[GenerateSqidsLegacyOverloads] // Will use source generation to add legacy overloads when targetting .NET Framework 4.6.1+ and up to .NET 6
public sealed partial class SqidsEncoder
#endif
{
	private const int MinAlphabetLength = 3;
	private const int MaxMinLength = 255;
	private const int MaxStackallocSize = 256; // NOTE: In bytes — this value is essentially arbitrary, the Microsoft docs is using 1024 but recommends being more conservative when choosing the value (https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/stackalloc), Hashids apparently uses 512 (https://github.com/ullmark/hashids.net/blob/9b1c69de4eedddf9d352c96117d8122af202e90f/src/Hashids.net/Hashids.cs#L17), and this article (https://vcsjones.dev/stackalloc/) uses 256. I've tried to be pretty cautious and gone with a low value.

	private readonly char[] _alphabet;
	private readonly int _minLength;
	private readonly string[] _blockList;

#if NET7_0_OR_GREATER
	/// <summary>
	/// Initializes a new instance of <see cref="SqidsEncoder{T}" /> with the default options.
	/// </summary>
	public SqidsEncoder() : this(new()) { }
	/// <summary>
	/// Initializes a new instance of <see cref="SqidsEncoder{T}" /> with custom options.
	/// </summary>
	/// <param name="options">
	/// The custom options.
	/// All properties of <see cref="SqidsOptions" /> are optional and will fall back to their
	/// defaults if not explicitly set.
	/// </param>
	/// <exception cref="T:System.ArgumentNullException" />
	/// <exception cref="T:System.ArgumentOutOfRangeException" />
	public SqidsEncoder(SqidsOptions options)
#else
	/// <summary>
	/// Initializes a new instance of <see cref="SqidsEncoder" /> with the default options.
	/// </summary>
	public SqidsEncoder() : this(new SqidsOptions()) { }
	/// <summary>
	/// Initializes a new instance of <see cref="SqidsEncoder" /> with custom options.
	/// </summary>
	/// <param name="options">
	/// The custom options.
	/// All properties of <see cref="SqidsOptions" /> are optional and will fall back to their
	/// defaults if not explicitly set.
	/// </param>
	/// <exception cref="T:System.ArgumentNullException" />
	/// <exception cref="T:System.ArgumentOutOfRangeException" />
	public SqidsEncoder(SqidsOptions options)
#endif
	{
		_ = options ?? throw new ArgumentNullException(nameof(options));
		_ = options.Alphabet ?? throw new ArgumentNullException(nameof(options.Alphabet));
		_ = options.BlockList ?? throw new ArgumentNullException(nameof(options.BlockList));

		if (options.Alphabet.Distinct().Count() != options.Alphabet.Length)
			throw new ArgumentOutOfRangeException(
				nameof(options.Alphabet),
				"The alphabet must not contain duplicate characters."
			);

		if (Encoding.UTF8.GetByteCount(options.Alphabet) != options.Alphabet.Length)
			throw new ArgumentOutOfRangeException(
				nameof(options.Alphabet),
				"The alphabet must not contain multi-byte characters."
			);

		if (options.Alphabet.Length < MinAlphabetLength)
			throw new ArgumentOutOfRangeException(
				nameof(options.Alphabet),
				$"The alphabet must contain at least {MinAlphabetLength} characters."
			);

		if (options.MinLength < 0 || options.MinLength > MaxMinLength)
			throw new ArgumentOutOfRangeException(
				nameof(options.MinLength),
				$"The minimum length must be between 0 and {MaxMinLength}."
			);

		_minLength = options.MinLength;

		// NOTE: Cleanup the blocklist:
		HashSet<string> blockList = new HashSet<string>(
			StringComparer.OrdinalIgnoreCase // NOTE: Effectively removes items that differ only in casing — leaves one version of each word casing-wise which will then be compared against the generated IDs case-insensitively
		);

		foreach (string w in options.BlockList)
		{
			if (
				// NOTE: Removes words that are less than 3 characters long
				w.Length < 3 ||
				// NOTE: Removes words that contain characters not found in the alphabet
#if NETSTANDARD2_0
				w.Any(c => options.Alphabet.IndexOf(c.ToString(), StringComparison.OrdinalIgnoreCase) == -1)) // NOTE: A `string.Contains` overload with `StringComparison` didn't exist prior to .NET Standard 2.1, so we have to resort to `IndexOf` — see https://stackoverflow.com/a/52791476
#else
				w.Any(c => !options.Alphabet.Contains(c, StringComparison.OrdinalIgnoreCase)))
#endif
			continue;

			blockList.Add(w);
		}

		_blockList = [..blockList]; // NOTE: Arrays are faster to iterate than HashSets, so we construct an array here.

		_alphabet = options.Alphabet.ToCharArray();
		ConsistentShuffle(_alphabet);
	}

#if NET7_0_OR_GREATER
	/// <summary>
	/// Encodes a single number into a Sqids ID.
	/// </summary>
	/// <param name="number">The number to encode.</param>
	/// <returns>A string containing the encoded ID.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">If the number passed is smaller than 0 (i.e. negative).</exception>
	/// <exception cref="T:System.ArgumentException">If the encoding reaches maximum re-generation attempts due to the blocklist.</exception>
	public string Encode(T number)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfLessThan(number, T.Zero, nameof(number));
#else
		if (number < T.Zero)
			throw new ArgumentOutOfRangeException(
				nameof(number),
				"Encoding is only supported for zero and positive numbers."
			);

#endif
		return EncodeCore([number]);  // NOTE: Collection initialization takes care of `stackalloc` intricacies
	}

	/// <summary>
	/// Encodes multiple numbers into a Sqids ID.
	/// </summary>
	/// <param name="numbers">The numbers to encode.</param>
	/// <returns>A string containing the encoded IDs, or an empty string if the array passed is empty.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">If any of the numbers passed is smaller than 0 (i.e. negative).</exception>
	/// <exception cref="T:System.ArgumentException">If the encoding reaches maximum re-generation attempts due to the blocklist.</exception>
	public string Encode(params T[] numbers)
	{
		if (numbers.Length == 0)
			return string.Empty;

		foreach (var number in numbers)
#if NET8_0_OR_GREATER
			ArgumentOutOfRangeException.ThrowIfLessThan(number, T.Zero, nameof(numbers));
#else
			if (number < T.Zero)
				throw new ArgumentOutOfRangeException(
					nameof(numbers),
					"Encoding is only supported for zero and positive numbers."
				);
#endif

		return EncodeCore(numbers.AsSpan());
	}

	/// <summary>
	/// Encodes a collection of numbers into a Sqids ID.
	/// </summary>
	/// <param name="numbers">The numbers to encode.</param>
	/// <returns>A string containing the encoded IDs, or an empty string if the `IEnumerable` passed is empty.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">If any of the numbers passed is smaller than 0 (i.e. negative).</exception>
	/// <exception cref="T:System.ArgumentException">If the encoding reaches maximum re-generation attempts due to the blocklist.</exception>
	public string Encode(IEnumerable<T> numbers) => Encode([.. numbers]);

	/// <summary>
	/// Decodes an ID into numbers.
	/// </summary>
	/// <param name="id">The encoded ID.</param>
	/// <returns>
	/// An array containing the decoded number(s) (it would contain only one element
	/// if the ID represents a single number); or an empty array if the input ID is null,
	/// empty, or includes characters not found in the alphabet.
	/// </returns>
	public IReadOnlyList<T> Decode(ReadOnlySpan<char> id) => DecodeCore(id);

#else
	// LEGACY API (Minimal overloads accepting any integral type and converting to ulong/"T")

	// Note: Overloads are source generated in a partial class.
	//       Since we use params[] arrays for the Encode methods, we don't need overloads for single values

	// Encoding/decoding overloads for int are kept here for "readability" purposes.

	/// <summary>Encodes one or more numbers into a Sqids ID.</summary>
	/// <param name="numbers">The number or numbers to encode.</param>
	/// <returns>A string containing the encoded IDs, or an empty string if the `IEnumerable` passed is empty.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">If any of the numbers passed is smaller than 0 (i.e. negative).</exception>
	/// <exception cref="T:System.ArgumentException">If the encoding reaches maximum re-generation attempts due to the blocklist.</exception>
	public string Encode(params int[] numbers) => EncodeCore([.. numbers.Select(n => (T)Check(n))]);

	/// <summary>Encodes a collection of numbers into a Sqids ID.</summary>
	/// <param name="numbers">The numbers to encode.</param>
	/// <returns>A string containing the encoded IDs, or an empty string if the `IEnumerable` passed is empty.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">If any of the numbers passed is smaller than 0 (i.e. negative).</exception>
	/// <exception cref="T:System.ArgumentException">If the encoding reaches maximum re-generation attempts due to the blocklist.</exception>
	public string Encode(IEnumerable<int> numbers) => EncodeCore([.. numbers.Select(n => (T)Check(n))]);

	// For backwards compatibility, the default Decode methods still return int lists

	/// <summary>Decodes an ID into numbers.</summary>
	/// <param name="id">The encoded ID.</param>
	/// <returns>
	/// An array containing the decoded number(s) (it would contain only one element
	/// if the ID represents a single number); or an empty array if the input ID is null,
	/// empty, or includes characters not found in the alphabet.
	/// </returns>
	public IReadOnlyList<int> Decode(ReadOnlySpan<char> id) => [..DecodeCore(id).Select(i => (int)i)];

	/// <summary>Decodes an ID into numbers.</summary>
	/// <param name="id">The encoded ID.</param>
	/// <returns>
	/// An array containing the decoded number(s) (it would contain only one element
	/// if the ID represents a single number); or an empty array if the input ID is null,
	/// empty, or includes characters not found in the alphabet.
	/// </returns>
	public IReadOnlyList<int> Decode(string id) => [..DecodeCore(id.AsSpan()).Select(i => (int)i)];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static long Check(long n) {
		if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Encoding is only supported for zero and positive numbers.");
		return n;
	}
#endif

	// Encode/DecodeCore shares the same logic for legacy and modern (.NET 7+) implementations
	private string EncodeCore(ReadOnlySpan<T> numbers, int increment = 0)
	{
		if (numbers.Length == 0)
			return string.Empty;

		if (increment > _alphabet.Length)
			throw new ArgumentException("Reached max attempts to re-generate the ID.");

		int offset = 0;
		for (int i = 0; i < numbers.Length; i++)
#if NET7_0_OR_GREATER
			offset += _alphabet[int.CreateChecked(numbers[i] % T.CreateChecked(_alphabet.Length))] + i;
#else
			offset += _alphabet[(int)(numbers[i] % (ulong)_alphabet.Length)] + i;
#endif

		offset = (numbers.Length + offset) % _alphabet.Length;
		offset = (offset + increment) % _alphabet.Length;

		Span<char> alphabetTemp = _alphabet.Length * sizeof(char) > MaxStackallocSize // NOTE: We multiply the number of characters by the size of a `char` to get the actual amount of memory that would be allocated.
			? new char[_alphabet.Length]
			: stackalloc char[_alphabet.Length];
		var alphabetSpan = _alphabet.AsSpan();
		alphabetSpan[offset..].CopyTo(alphabetTemp[..^offset]);
		alphabetSpan[..offset].CopyTo(alphabetTemp[^offset..]);

		char prefix = alphabetTemp[0];
		alphabetTemp.Reverse();

		var builder = new StringBuilder(); // TODO: pool a la Hashids.net?
		builder.Append(prefix);

		for (int i = 0; i < numbers.Length; i++)
		{
			var number = numbers[i];
			var alphabetWithoutSeparator = alphabetTemp[1..]; // NOTE: Excludes the first character — which is the separator
			var encodedNumber = ToId(number, alphabetWithoutSeparator);
			builder.Append(encodedNumber);

			if (i >= numbers.Length - 1) // NOTE: If the last one
				continue;

			char separator = alphabetTemp[0];
			builder.Append(separator);
			ConsistentShuffle(alphabetTemp);
		}

		if (builder.Length < _minLength)
		{
			char separator = alphabetTemp[0];
			builder.Append(separator);

			while (builder.Length < _minLength)
			{
				ConsistentShuffle(alphabetTemp);
				int toIndex = Math.Min(_minLength - builder.Length, _alphabet.Length);
				builder.Append(alphabetTemp[..toIndex]);
			}
		}

		string result = builder.ToString();

		if (IsBlockedId(result.AsSpan()))
			result = EncodeCore(numbers, increment + 1);

		return result;
	}

#if NET8_0_OR_GREATER
	private List<T> DecodeCore(ReadOnlySpan<char> id)
#else
	private IReadOnlyList<T> DecodeCore(ReadOnlySpan<char> id)
#endif
	{
		if (id.IsEmpty)
			return [];

		foreach (char c in id)
			if (!_alphabet.Contains(c))
				return [];

		var alphabetSpan = _alphabet.AsSpan();

		char prefix = id[0];
		int offset = alphabetSpan.IndexOf(prefix);

		Span<char> alphabetTemp = _alphabet.Length * sizeof(char) > MaxStackallocSize
			? new char[_alphabet.Length]
			: stackalloc char[_alphabet.Length];
		alphabetSpan[offset..].CopyTo(alphabetTemp[..^offset]);
		alphabetSpan[..offset].CopyTo(alphabetTemp[^offset..]);

		alphabetTemp.Reverse();

		id = id[1..]; // NOTE: Exclude the prefix
		var result = new List<T>();
		while (!id.IsEmpty)
		{
			char separator = alphabetTemp[0];

			var separatorIndex = id.IndexOf(separator);
			var chunk = separatorIndex == -1 ? id : id[..separatorIndex]; // NOTE: The first part of `id` (every thing to the left of the separator) represents the number that we ought to decode.
			id = separatorIndex == -1 ? default : id[(separatorIndex + 1)..]; // NOTE: Everything to the right of the separator will be `id` for the next iteration

			if (chunk.IsEmpty)
				return result;

			var alphabetWithoutSeparator = alphabetTemp[1..]; // NOTE: Exclude the first character — which is the separator
			var decodedNumber = ToNumber(chunk, alphabetWithoutSeparator);
			result.Add(decodedNumber);

			if (!id.IsEmpty)
				ConsistentShuffle(alphabetTemp);
		}

		return result;
	}

	private bool IsBlockedId(ReadOnlySpan<char> id)
	{
		foreach (string word in _blockList)
		{
			if (word.Length > id.Length)
				continue;

			if ((id.Length <= 3 || word.Length <= 3) &&
				id.Equals(word.AsSpan(), StringComparison.OrdinalIgnoreCase))
				return true;

			if (word.Any(char.IsDigit) &&
				(id.StartsWith(word.AsSpan(), StringComparison.OrdinalIgnoreCase) ||
				 id.EndsWith(word.AsSpan(), StringComparison.OrdinalIgnoreCase)))
				return true;

			if (id.Contains(word.AsSpan(), StringComparison.OrdinalIgnoreCase))
				return true;
		}

		return false;
	}

	// NOTE: Shuffles a span of characters in place. The shuffle produces consistent results.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ConsistentShuffle(Span<char> chars)
	{
		for (int i = 0, j = chars.Length - 1; j > 0; i++, j--)
		{
			int r = (i * j + chars[i] + chars[j]) % chars.Length;
			(chars[i], chars[r]) = (chars[r], chars[i]);
		}
	}

	private static ReadOnlySpan<char> ToId(T num, ReadOnlySpan<char> alphabet)
	{
		var id = new StringBuilder();
		var result = num;

#if NET7_0_OR_GREATER
		do
		{
			id.Insert(0, alphabet[int.CreateChecked(result % T.CreateChecked(alphabet.Length))]);
			result /= T.CreateChecked(alphabet.Length);
		} while (result > T.Zero);
#else
		ulong alphabetLength = (ulong)alphabet.Length;
		do
		{
			id.Insert(0, alphabet[(int)(result % alphabetLength)]);
			result /= alphabetLength;
		} while (result > 0);
#endif

		return id.ToString().AsSpan(); // TODO: possibly avoid creating a string
	}

	private static T ToNumber(ReadOnlySpan<char> id, ReadOnlySpan<char> alphabet)
	{
#if NET7_0_OR_GREATER
		T result = T.Zero;
		foreach (var character in id)
			result = result * T.CreateChecked(alphabet.Length) + T.CreateChecked(alphabet.IndexOf(character));
#else
		T result = 0;
		ulong alphabetLength = (ulong)alphabet.Length;
		foreach (var character in id)
			result = result * alphabetLength + (ulong)alphabet.IndexOf(character);
#endif
		return result;
	}
}

