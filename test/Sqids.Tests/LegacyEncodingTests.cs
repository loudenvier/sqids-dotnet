#if !NET7_0_OR_GREATER

namespace Sqids.Tests;

public class LegacyEncodingTests
{
	// NOTE: We replicate each original test of encoding and decoding for each integral type
	//       Superfluous tests (e.g., for int and negativity) are omitted.

	[Test]
	public void EncodeAndDecode_SingleNumbers_AllIntegralTypes_ReturnsExactMatch() {
		var sqids = new SqidsEncoder();
		string idB  = sqids.Encode(byte.MaxValue);
		string idSB = sqids.Encode(sbyte.MaxValue); 
		string idS  = sqids.Encode(short.MaxValue);
		string idUS = sqids.Encode(ushort.MaxValue);
		string idI  = sqids.Encode(int.MaxValue);
		string idUI = sqids.Encode(uint.MaxValue);
		string idL	= sqids.Encode(long.MaxValue);
		string idUL = sqids.Encode(ulong.MaxValue); // Non CLS-Compliant
		byte   bID  = sqids.DecodeByte(idB).Single();
		sbyte  sbID = sqids.DecodeSByte(idSB).Single(); // Non CLS-Compliant
		short  sID  = sqids.DecodeShort(idS).Single();
		ushort usID = sqids.DecodeUShort(idUS).Single(); // Non CLS-Compliant
		int	   iID  = sqids.Decode(idI).Single(); // or DecodeInt() 
		uint   uiID = sqids.DecodeUInt(idUI).Single(); // Non CLS-Compliant
		long   lID  = sqids.DecodeLong(idL).Single();
		ulong  ulID = sqids.DecodeULong(idUL).Single(); // Non CLS-Compliant
		bID.ShouldBe(byte.MaxValue);
		sbID.ShouldBe(sbyte.MaxValue);
		sID.ShouldBe(short.MaxValue);
		usID.ShouldBe(ushort.MaxValue);
		iID.ShouldBe(int.MaxValue);
		uiID.ShouldBe(uint.MaxValue);
		lID.ShouldBe(long.MaxValue);
		ulID.ShouldBe(ulong.MaxValue);
	}

		[Test]
	public void EncodeAndDecode_MultipleNumbers_AllIntegralTypes_ReturnsExactMatch() {
		var sqids = new SqidsEncoder();
		string idB  = sqids.Encode((byte[])[byte.MaxValue, 1]);
		string idSB = sqids.Encode(sbyte.MaxValue, (sbyte)1); 
		string idS  = sqids.Encode(short.MaxValue, (short)1);
		string idUS = sqids.Encode(ushort.MaxValue, (ushort)1);
		string idI  = sqids.Encode(int.MaxValue, 1);
		string idUI = sqids.Encode(uint.MaxValue, 1);
		string idL	= sqids.Encode(long.MaxValue, 1);
		string idUL = sqids.Encode(ulong.MaxValue, 1); // Non CLS-Compliant
		IEnumerable<byte>   bID  = sqids.DecodeByte(idB);
		IEnumerable<sbyte>  sbID = sqids.DecodeSByte(idSB); // Non CLS-Compliant
		IEnumerable<short>  sID  = sqids.DecodeShort(idS);
		IEnumerable<ushort> usID = sqids.DecodeUShort(idUS); // Non CLS-Compliant
		IEnumerable<int>    iID  = sqids.Decode(idI); // or DecodeInt() 
		IEnumerable<uint>   uiID = sqids.DecodeUInt(idUI); // Non CLS-Compliant
		IEnumerable<long>   lID  = sqids.DecodeLong(idL);
		IEnumerable<ulong>  ulID = sqids.DecodeULong(idUL); // Non CLS-Compliant
		bID .ShouldBe([byte.MaxValue, (byte)1]);
		sbID.ShouldBe([sbyte.MaxValue, (sbyte)1]);
		sID .ShouldBe([short.MaxValue, (short)1]);
		usID.ShouldBe([ushort.MaxValue, (ushort)1]);
		iID .ShouldBe([int.MaxValue, 1]);
		uiID.ShouldBe([uint.MaxValue, 1U]);
		lID .ShouldBe([long.MaxValue, 1L]);
		ulID.ShouldBe([ulong.MaxValue, 1UL]);
	}


	#region ULONG Tests

	// NOTE: Incremental
	[TestCase(0UL, "bM")]
	[TestCase(1UL, "Uk")]
	[TestCase(2UL, "gb")]
	[TestCase(3UL, "Ef")]
	[TestCase(4UL, "Vq")]
	[TestCase(5UL, "uw")]
	[TestCase(6UL, "OI")]
	[TestCase(7UL, "AX")]
	[TestCase(8UL, "p6")]
	[TestCase(9UL, "nJ")]
	public void EncodeAndDecodeULong_SingleNumber_ReturnsExactMatch(ulong number, string id)
	{
		var sqids = new SqidsEncoder();
		sqids.Encode(number).ShouldBe(id);
		sqids.DecodeULong(id).ShouldBe([number]);
	}

	// NOTE: Simple case
	[TestCase(new ulong[] { 1, 2, 3 }, "86Rf07")]
	// NOTE: Incremental
	[TestCase(new ulong[] { 0, 0 }, "SvIz")]
	[TestCase(new ulong[] { 0, 1 }, "n3qa")]
	[TestCase(new ulong[] { 0, 2 }, "tryF")]
	[TestCase(new ulong[] { 0, 3 }, "eg6q")]
	[TestCase(new ulong[] { 0, 4 }, "rSCF")]
	[TestCase(new ulong[] { 0, 5 }, "sR8x")]
	[TestCase(new ulong[] { 0, 6 }, "uY2M")]
	[TestCase(new ulong[] { 0, 7 }, "74dI")]
	[TestCase(new ulong[] { 0, 8 }, "30WX")]
	[TestCase(new ulong[] { 0, 9 }, "moxr")]
	// NOTE: Incremental
	[TestCase(new ulong[] { 0, 0 }, "SvIz")]
	[TestCase(new ulong[] { 1, 0 }, "nWqP")]
	[TestCase(new ulong[] { 2, 0 }, "tSyw")]
	[TestCase(new ulong[] { 3, 0 }, "eX68")]
	[TestCase(new ulong[] { 4, 0 }, "rxCY")]
	[TestCase(new ulong[] { 5, 0 }, "sV8a")]
	[TestCase(new ulong[] { 6, 0 }, "uf2K")]
	[TestCase(new ulong[] { 7, 0 }, "7Cdk")]
	[TestCase(new ulong[] { 8, 0 }, "3aWP")]
	[TestCase(new ulong[] { 9, 0 }, "m2xn")]
	// NOTE: Empty array should encode into empty string
	[TestCase(new ulong[] { }, "")]
	public void EncodeAndDecodeULong_MultipleNumbers_ReturnsExactMatch(ulong[] numbers, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(numbers).ShouldBe(id);
		sqids.Encode(numbers.ToList()).ShouldBe(id); // NOTE: Selects the `IEnumerable<int>` overload
		sqids.DecodeULong(id).ShouldBe(numbers);
	}

	[TestCase(new ulong[] { 0, 0, 0, 1, 2, 3, 100, 1_000, 100_000, 1_000_000, ulong.MaxValue })]
	[TestCase(new ulong[]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
		48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
		71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
		94, 95, 96, 97, 98, 99
	})]
	public void EncodeAndDecode_MultipleNumbers_RoundTripsSuccessfully(ulong[] numbers) {
		var sqids = new SqidsEncoder();

		sqids.DecodeULong(sqids.Encode(numbers)).ShouldBe(numbers);
	}
	#endregion

	#region LONG Tests

	// NOTE: Incremental
	[TestCase(0, "bM")]
	[TestCase(1, "Uk")]
	[TestCase(2, "gb")]
	[TestCase(3, "Ef")]
	[TestCase(4, "Vq")]
	[TestCase(5, "uw")]
	[TestCase(6, "OI")]
	[TestCase(7, "AX")]
	[TestCase(8, "p6")]
	[TestCase(9, "nJ")]
	public void EncodeAndDecodeLong_SingleNumber_ReturnsExactMatch(long number, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(number).ShouldBe(id);
		sqids.DecodeLong(id).ShouldBe([number]);
	}


	// NOTE: Simple case
	[TestCase(new long[] { 1, 2, 3 }, "86Rf07")]
	// NOTE: Incremental
	[TestCase(new long[] { 0, 0 }, "SvIz")]
	[TestCase(new long[] { 0, 1 }, "n3qa")]
	[TestCase(new long[] { 0, 2 }, "tryF")]
	[TestCase(new long[] { 0, 3 }, "eg6q")]
	[TestCase(new long[] { 0, 4 }, "rSCF")]
	[TestCase(new long[] { 0, 5 }, "sR8x")]
	[TestCase(new long[] { 0, 6 }, "uY2M")]
	[TestCase(new long[] { 0, 7 }, "74dI")]
	[TestCase(new long[] { 0, 8 }, "30WX")]
	[TestCase(new long[] { 0, 9 }, "moxr")]
	// NOTE: Incremental
	[TestCase(new long[] { 0, 0 }, "SvIz")]
	[TestCase(new long[] { 1, 0 }, "nWqP")]
	[TestCase(new long[] { 2, 0 }, "tSyw")]
	[TestCase(new long[] { 3, 0 }, "eX68")]
	[TestCase(new long[] { 4, 0 }, "rxCY")]
	[TestCase(new long[] { 5, 0 }, "sV8a")]
	[TestCase(new long[] { 6, 0 }, "uf2K")]
	[TestCase(new long[] { 7, 0 }, "7Cdk")]
	[TestCase(new long[] { 8, 0 }, "3aWP")]
	[TestCase(new long[] { 9, 0 }, "m2xn")]
	// NOTE: Empty array should encode into empty string
	[TestCase(new long[] { }, "")]
	public void EncodeAndDecodeLong_MultipleNumbers_ReturnsExactMatch(long[] numbers, string id)
	{
		var sqids = new SqidsEncoder();
		sqids.Encode(numbers).ShouldBe(id);
		sqids.Encode(numbers.ToList()).ShouldBe(id); // NOTE: Selects the `IEnumerable<int>` overload
		sqids.DecodeLong(id).ShouldBe(numbers);
	}

	[TestCase(new long[] { 0, 0, 0, 1, 2, 3, 100, 1_000, 100_000, 1_000_000, long.MaxValue })]
	[TestCase(new long[]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
		48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
		71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
		94, 95, 96, 97, 98, 99
	})]
	public void EncodeAndDecode_MultipleNumbers_RoundTripsSuccessfully(long[] numbers)
	{
		var sqids = new SqidsEncoder();

		sqids.DecodeLong(sqids.Encode(numbers)).ShouldBe(numbers);
	}
	#endregion

	#region UINT Tests

	// NOTE: Incremental
	[TestCase(0U, "bM")]
	[TestCase(1U, "Uk")]
	[TestCase(2U, "gb")]
	[TestCase(3U, "Ef")]
	[TestCase(4U, "Vq")]
	[TestCase(5U, "uw")]
	[TestCase(6U, "OI")]
	[TestCase(7U, "AX")]
	[TestCase(8U, "p6")]
	[TestCase(9U, "nJ")]
	public void EncodeAndDecodeUInt_SingleNumber_ReturnsExactMatch(uint number, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(number).ShouldBe(id);
		sqids.DecodeUInt(id).ShouldBe([number]);
	}

	// NOTE: Simple case
	[TestCase(new uint[] { 1, 2, 3 }, "86Rf07")]
	// NOTE: Incremental
	[TestCase(new uint[] { 0, 0 }, "SvIz")]
	[TestCase(new uint[] { 0, 1 }, "n3qa")]
	[TestCase(new uint[] { 0, 2 }, "tryF")]
	[TestCase(new uint[] { 0, 3 }, "eg6q")]
	[TestCase(new uint[] { 0, 4 }, "rSCF")]
	[TestCase(new uint[] { 0, 5 }, "sR8x")]
	[TestCase(new uint[] { 0, 6 }, "uY2M")]
	[TestCase(new uint[] { 0, 7 }, "74dI")]
	[TestCase(new uint[] { 0, 8 }, "30WX")]
	[TestCase(new uint[] { 0, 9 }, "moxr")]
	// NOTE: Incremental
	[TestCase(new uint[] { 0, 0 }, "SvIz")]
	[TestCase(new uint[] { 1, 0 }, "nWqP")]
	[TestCase(new uint[] { 2, 0 }, "tSyw")]
	[TestCase(new uint[] { 3, 0 }, "eX68")]
	[TestCase(new uint[] { 4, 0 }, "rxCY")]
	[TestCase(new uint[] { 5, 0 }, "sV8a")]
	[TestCase(new uint[] { 6, 0 }, "uf2K")]
	[TestCase(new uint[] { 7, 0 }, "7Cdk")]
	[TestCase(new uint[] { 8, 0 }, "3aWP")]
	[TestCase(new uint[] { 9, 0 }, "m2xn")]
	// NOTE: Empty array should encode into empty string
	[TestCase(new uint[] { }, "")]
	public void EncodeAndDecodeUInt_MultipleNumbers_ReturnsExactMatch(uint[] numbers, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(numbers).ShouldBe(id);
		sqids.Encode(numbers.ToList()).ShouldBe(id); // NOTE: Selects the `IEnumerable<int>` overload
		sqids.DecodeUInt(id).ShouldBe(numbers);
	}

	[TestCase(new uint[] { 0, 0, 0, 1, 2, 3, 100, 1_000, 100_000, 1_000_000, uint.MaxValue })]
	[TestCase(new uint[]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
		48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
		71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
		94, 95, 96, 97, 98, 99
	})]
	public void EncodeAndDecode_MultipleNumbers_RoundTripsSuccessfully(uint[] numbers) {
		var sqids = new SqidsEncoder();

		sqids.DecodeUInt(sqids.Encode(numbers)).ShouldBe(numbers);
	}
	#endregion

	#region SHORT Tests

	// NOTE: Incremental
	[TestCase(0, "bM")]
	[TestCase(1, "Uk")]
	[TestCase(2, "gb")]
	[TestCase(3, "Ef")]
	[TestCase(4, "Vq")]
	[TestCase(5, "uw")]
	[TestCase(6, "OI")]
	[TestCase(7, "AX")]
	[TestCase(8, "p6")]
	[TestCase(9, "nJ")]
	public void EncodeAndDecodeShort_SingleNumber_ReturnsExactMatch(short number, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(number).ShouldBe(id);
		sqids.DecodeShort(id).ShouldBe([number]);
	}

	// NOTE: Simple case
	[TestCase(new short[] { 1, 2, 3 }, "86Rf07")]
	// NOTE: Incremental
	[TestCase(new short[] { 0, 0 }, "SvIz")]
	[TestCase(new short[] { 0, 1 }, "n3qa")]
	[TestCase(new short[] { 0, 2 }, "tryF")]
	[TestCase(new short[] { 0, 3 }, "eg6q")]
	[TestCase(new short[] { 0, 4 }, "rSCF")]
	[TestCase(new short[] { 0, 5 }, "sR8x")]
	[TestCase(new short[] { 0, 6 }, "uY2M")]
	[TestCase(new short[] { 0, 7 }, "74dI")]
	[TestCase(new short[] { 0, 8 }, "30WX")]
	[TestCase(new short[] { 0, 9 }, "moxr")]
	// NOTE: Incremental
	[TestCase(new short[] { 0, 0 }, "SvIz")]
	[TestCase(new short[] { 1, 0 }, "nWqP")]
	[TestCase(new short[] { 2, 0 }, "tSyw")]
	[TestCase(new short[] { 3, 0 }, "eX68")]
	[TestCase(new short[] { 4, 0 }, "rxCY")]
	[TestCase(new short[] { 5, 0 }, "sV8a")]
	[TestCase(new short[] { 6, 0 }, "uf2K")]
	[TestCase(new short[] { 7, 0 }, "7Cdk")]
	[TestCase(new short[] { 8, 0 }, "3aWP")]
	[TestCase(new short[] { 9, 0 }, "m2xn")]
	// NOTE: Empty array should encode into empty string
	[TestCase(new short[] { }, "")]
	public void EncodeAndDecodeShort_MultipleNumbers_ReturnsExactMatch(short[] numbers, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(numbers).ShouldBe(id);
		sqids.Encode(numbers.ToList()).ShouldBe(id); // NOTE: Selects the `IEnumerable<int>` overload
		sqids.DecodeShort(id).ShouldBe(numbers);
	}

	[TestCase(new short[] { 0, 0, 0, 1, 2, 3, 100, 1_000, short.MaxValue })]
	[TestCase(new short[]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
		48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
		71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
		94, 95, 96, 97, 98, 99
	})]
	public void EncodeAndDecode_MultipleNumbers_RoundTripsSuccessfully(short[] numbers) {
		var sqids = new SqidsEncoder();

		sqids.DecodeShort(sqids.Encode(numbers)).ShouldBe(numbers);
	}
	#endregion

	#region USHORT Tests

	// NOTE: Incremental
	[TestCase((ushort)0, "bM")]
	[TestCase((ushort)1, "Uk")]
	[TestCase((ushort)2, "gb")]
	[TestCase((ushort)3, "Ef")]
	[TestCase((ushort)4, "Vq")]
	[TestCase((ushort)5, "uw")]
	[TestCase((ushort)6, "OI")]
	[TestCase((ushort)7, "AX")]
	[TestCase((ushort)8, "p6")]
	[TestCase((ushort)9, "nJ")]
	public void EncodeAndDecodeUShort_SingleNumber_ReturnsExactMatch(ushort number, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(number).ShouldBe(id);
		sqids.DecodeUShort(id).ShouldBe([number]);
	}

	// NOTE: Simple case
	[TestCase(new ushort[] { 1, 2, 3 }, "86Rf07")]
	// NOTE: Incremental
	[TestCase(new ushort[] { 0, 0 }, "SvIz")]
	[TestCase(new ushort[] { 0, 1 }, "n3qa")]
	[TestCase(new ushort[] { 0, 2 }, "tryF")]
	[TestCase(new ushort[] { 0, 3 }, "eg6q")]
	[TestCase(new ushort[] { 0, 4 }, "rSCF")]
	[TestCase(new ushort[] { 0, 5 }, "sR8x")]
	[TestCase(new ushort[] { 0, 6 }, "uY2M")]
	[TestCase(new ushort[] { 0, 7 }, "74dI")]
	[TestCase(new ushort[] { 0, 8 }, "30WX")]
	[TestCase(new ushort[] { 0, 9 }, "moxr")]
	// NOTE: Incremental
	[TestCase(new ushort[] { 0, 0 }, "SvIz")]
	[TestCase(new ushort[] { 1, 0 }, "nWqP")]
	[TestCase(new ushort[] { 2, 0 }, "tSyw")]
	[TestCase(new ushort[] { 3, 0 }, "eX68")]
	[TestCase(new ushort[] { 4, 0 }, "rxCY")]
	[TestCase(new ushort[] { 5, 0 }, "sV8a")]
	[TestCase(new ushort[] { 6, 0 }, "uf2K")]
	[TestCase(new ushort[] { 7, 0 }, "7Cdk")]
	[TestCase(new ushort[] { 8, 0 }, "3aWP")]
	[TestCase(new ushort[] { 9, 0 }, "m2xn")]
	// NOTE: Empty array should encode into empty string
	[TestCase(new ushort[] { }, "")]
	public void EncodeAndDecodeUShort_MultipleNumbers_ReturnsExactMatch(ushort[] numbers, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(numbers).ShouldBe(id);
		sqids.Encode(numbers.ToList()).ShouldBe(id); // NOTE: Selects the `IEnumerable<int>` overload
		sqids.DecodeUShort(id).ShouldBe(numbers);
	}

	[TestCase(new ushort[] { 0, 0, 0, 1, 2, 3, 100, 1_000, ushort.MaxValue })]
	[TestCase(new ushort[]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
		48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
		71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
		94, 95, 96, 97, 98, 99
	})]
	public void EncodeAndDecode_MultipleNumbers_RoundTripsSuccessfully(ushort[] numbers) {
		var sqids = new SqidsEncoder();

		sqids.DecodeUShort(sqids.Encode(numbers)).ShouldBe(numbers);
	}
	#endregion

	#region BYTE Tests

	// NOTE: Incremental
	[TestCase((byte)0, "bM")]
	[TestCase((byte)1, "Uk")]
	[TestCase((byte)2, "gb")]
	[TestCase((byte)3, "Ef")]
	[TestCase((byte)4, "Vq")]
	[TestCase((byte)5, "uw")]
	[TestCase((byte)6, "OI")]
	[TestCase((byte)7, "AX")]
	[TestCase((byte)8, "p6")]
	[TestCase((byte)9, "nJ")]
	public void EncodeAndDecodeByte_SingleNumber_ReturnsExactMatch(byte number, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(number).ShouldBe(id);
		sqids.DecodeByte(id).ShouldBe([number]);
	}

	// NOTE: Simple case
	[TestCase(new byte[] { 1, 2, 3 }, "86Rf07")]
	// NOTE: Incremental
	[TestCase(new byte[] { 0, 0 }, "SvIz")]
	[TestCase(new byte[] { 0, 1 }, "n3qa")]
	[TestCase(new byte[] { 0, 2 }, "tryF")]
	[TestCase(new byte[] { 0, 3 }, "eg6q")]
	[TestCase(new byte[] { 0, 4 }, "rSCF")]
	[TestCase(new byte[] { 0, 5 }, "sR8x")]
	[TestCase(new byte[] { 0, 6 }, "uY2M")]
	[TestCase(new byte[] { 0, 7 }, "74dI")]
	[TestCase(new byte[] { 0, 8 }, "30WX")]
	[TestCase(new byte[] { 0, 9 }, "moxr")]
	// NOTE: Incremental
	[TestCase(new byte[] { 0, 0 }, "SvIz")]
	[TestCase(new byte[] { 1, 0 }, "nWqP")]
	[TestCase(new byte[] { 2, 0 }, "tSyw")]
	[TestCase(new byte[] { 3, 0 }, "eX68")]
	[TestCase(new byte[] { 4, 0 }, "rxCY")]
	[TestCase(new byte[] { 5, 0 }, "sV8a")]
	[TestCase(new byte[] { 6, 0 }, "uf2K")]
	[TestCase(new byte[] { 7, 0 }, "7Cdk")]
	[TestCase(new byte[] { 8, 0 }, "3aWP")]
	[TestCase(new byte[] { 9, 0 }, "m2xn")]
	// NOTE: Empty array should encode into empty string
	[TestCase(new byte[] { }, "")]
	public void EncodeAndDecodeByte_MultipleNumbers_ReturnsExactMatch(byte[] numbers, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(numbers).ShouldBe(id);
		sqids.Encode(numbers.ToList()).ShouldBe(id); // NOTE: Selects the `IEnumerable<int>` overload
		sqids.DecodeByte(id).ShouldBe(numbers);
	}

	[TestCase(new byte[] { 0, 0, 0, 1, 2, 3, 100, byte.MaxValue })]
	[TestCase(new byte[]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
		48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
		71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
		94, 95, 96, 97, 98, 99
	})]
	public void EncodeAndDecode_MultipleNumbers_RoundTripsSuccessfully(byte[] numbers) {
		var sqids = new SqidsEncoder();

		sqids.DecodeByte(sqids.Encode(numbers)).ShouldBe(numbers);
	}
	#endregion

	#region SBYTE Tests

	// NOTE: Incremental
	[TestCase((sbyte)0, "bM")]
	[TestCase((sbyte)1, "Uk")]
	[TestCase((sbyte)2, "gb")]
	[TestCase((sbyte)3, "Ef")]
	[TestCase((sbyte)4, "Vq")]
	[TestCase((sbyte)5, "uw")]
	[TestCase((sbyte)6, "OI")]
	[TestCase((sbyte)7, "AX")]
	[TestCase((sbyte)8, "p6")]
	[TestCase((sbyte)9, "nJ")]
	public void EncodeAndDecodeSByte_SingleNumber_ReturnsExactMatch(sbyte number, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(number).ShouldBe(id);
		sqids.DecodeSByte(id).ShouldBe([number]);
	}

	// NOTE: Simple case
	[TestCase(new sbyte[] { 1, 2, 3 }, "86Rf07")]
	// NOTE: Incremental
	[TestCase(new sbyte[] { 0, 0 }, "SvIz")]
	[TestCase(new sbyte[] { 0, 1 }, "n3qa")]
	[TestCase(new sbyte[] { 0, 2 }, "tryF")]
	[TestCase(new sbyte[] { 0, 3 }, "eg6q")]
	[TestCase(new sbyte[] { 0, 4 }, "rSCF")]
	[TestCase(new sbyte[] { 0, 5 }, "sR8x")]
	[TestCase(new sbyte[] { 0, 6 }, "uY2M")]
	[TestCase(new sbyte[] { 0, 7 }, "74dI")]
	[TestCase(new sbyte[] { 0, 8 }, "30WX")]
	[TestCase(new sbyte[] { 0, 9 }, "moxr")]
	// NOTE: Incremental
	[TestCase(new sbyte[] { 0, 0 }, "SvIz")]
	[TestCase(new sbyte[] { 1, 0 }, "nWqP")]
	[TestCase(new sbyte[] { 2, 0 }, "tSyw")]
	[TestCase(new sbyte[] { 3, 0 }, "eX68")]
	[TestCase(new sbyte[] { 4, 0 }, "rxCY")]
	[TestCase(new sbyte[] { 5, 0 }, "sV8a")]
	[TestCase(new sbyte[] { 6, 0 }, "uf2K")]
	[TestCase(new sbyte[] { 7, 0 }, "7Cdk")]
	[TestCase(new sbyte[] { 8, 0 }, "3aWP")]
	[TestCase(new sbyte[] { 9, 0 }, "m2xn")]
	// NOTE: Empty array should encode into empty string
	[TestCase(new sbyte[] { }, "")]
	public void EncodeAndDecodeSByte_MultipleNumbers_ReturnsExactMatch(sbyte[] numbers, string id) {
		var sqids = new SqidsEncoder();
		sqids.Encode(numbers).ShouldBe(id);
		sqids.Encode(numbers.ToList()).ShouldBe(id); // NOTE: Selects the `IEnumerable<int>` overload
		sqids.DecodeSByte(id).ShouldBe(numbers);
	}

	[TestCase(new sbyte[] { 0, 0, 0, 1, 2, 3, 100, sbyte.MaxValue })]
	[TestCase(new sbyte[]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
		25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
		48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
		71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
		94, 95, 96, 97, 98, 99
	})]
	public void EncodeAndDecode_MultipleNumbers_RoundTripsSuccessfully(sbyte[] numbers) {
		var sqids = new SqidsEncoder();

		sqids.DecodeSByte(sqids.Encode(numbers)).ShouldBe(numbers);
	}
	#endregion
}

#endif
