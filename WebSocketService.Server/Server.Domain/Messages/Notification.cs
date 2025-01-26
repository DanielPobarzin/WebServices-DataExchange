using Server.Domain.Library;

namespace Server.Domain.Messages
{
	/// <summary>
	/// Уведомление
	/// </summary>
	public sealed class Notification : Message
    {
		/// <summary>
		/// Передаваемое значение
		/// </summary>
		public double? Value { get; }

		/// <summary>
		/// Качество
		/// </summary>
		public bool Quality { get; }

        /// <inheritdoc cref="Notification"/>
        /// <param name="content">Сообщение</param>
        /// <param name="value">Передаваемое значение</param>
        /// <param name="quality">Парметр качества</param>
        public Notification(string? content, double? value, bool quality = false)
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