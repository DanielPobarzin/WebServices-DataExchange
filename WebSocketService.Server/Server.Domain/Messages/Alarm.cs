using Server.Domain.Library;

namespace Server.Domain.Messages
{
	/// <summary>
	/// Тревога
	/// </summary>
	public sealed class Alarm : Message
    {
		/// <summary>
		/// Передаваемое значение
		/// </summary>
		public double? Value { get; }

		/// <summary>
		/// Качество
		/// </summary>
		public bool Quality { get; }

	    /// <inheritdoc cref="Alarm"/>
	    /// <param name="content"></param>
	    /// <param name="value"></param>
	    /// <param name="quality"></param>
	    public Alarm(string? content, double? value, bool quality = false)
			: base(content ?? string.Empty)
		{
		    Value = value;
		    Quality = quality;
	    }

	    protected override IEnumerable<object?> GetEqualityAttributes()
	    {
		    yield return Value;
		    yield return Quality;
		    yield return Content;
		    yield return TimeStamp;
	    }
    }
}