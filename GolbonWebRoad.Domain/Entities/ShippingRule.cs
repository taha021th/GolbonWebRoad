
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    /// <summary>
    /// این کلاس قوانین قیمت‌گذاری برای یک روش ارسال را مشخص می‌کند.
    /// هر روش ارسال می‌تواند چندین قانون داشته باشد.
    /// </summary>
    public class ShippingRule
    {
        public int Id { get; set; }

        [Required]
        public int ShippingMethodId { get; set; }
        [ForeignKey(nameof(ShippingMethodId))]
        public virtual ShippingMethod ShippingMethod { get; set; }

        /// <summary>
        /// شرط اعمال این قانون (مثلا وزن، منطقه جغرافیایی و...).
        /// برای سادگی، فعلا از یک رشته استفاده می‌کنیم.
        /// مثال: \"Weight > 2000\" یا \"Province == Tehran\"
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// هزینه ثابت این قانون.
        /// </summary>
        public decimal BaseCost { get; set; }

        /// <summary>
        /// هزینه به ازای هر گرم اضافی (اگر اعمال شود).
        /// </summary>
        public decimal? CostPerGram { get; set; }

        /// <summary>
        /// هزینه به ازای هر سانتی‌متر مکعب اضافی (برای وزن حجمی).
        /// </summary>
        public decimal? CostPerCubicCentimeter { get; set; }

        public int Priority { get; set; } // اولویت اعمال قانون
    }
}
