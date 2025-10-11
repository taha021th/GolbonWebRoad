using System.ComponentModel.DataAnnotations;

namespace GolbonWebRoad.Domain.Entities
{
    /// <summary>
    /// روش‌های ارسال موجود در سیستم مثل پست، تیپاکس، باربری و پیک
    /// </summary>
    public class ShippingMethod
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // نام روش ارسال
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } // کد یکتا: POST, TIPAX, BARBARI, PEYK
        
        public string? Description { get; set; } // توضیحات اضافی
        
        public bool IsActive { get; set; } = true; // آیا این روش ارسال فعال است؟
        
        public int Priority { get; set; } // اولویت انتخاب (عدد کمتر = اولویت بالاتر)
        
        /// <summary>
        /// حداکثر وزن قابل ارسال به گرم
        /// </summary>
        public int MaxWeight { get; set; }
        
        /// <summary>
        /// حداکثر طول قابل ارسال به سانتی‌متر
        /// </summary>
        public decimal MaxLength { get; set; }
        
        /// <summary>
        /// حداکثر عرض قابل ارسال به سانتی‌متر
        /// </summary>
        public decimal MaxWidth { get; set; }
        
        /// <summary>
        /// حداکثر ارتفاع قابل ارسال به سانتی‌متر
        /// </summary>
        public decimal MaxHeight { get; set; }
        
        /// <summary>
        /// حداکثر حجم قابل ارسال به سانتی‌متر مکعب
        /// </summary>
        public decimal MaxVolume { get; set; }
        
        /// <summary>
        /// حداکثر قیمت محصول قابل ارسال (برای محصولات گران‌قیمت)
        /// </summary>
        public decimal? MaxValue { get; set; }
        
        /// <summary>
        /// حداقل قیمت محصول قابل ارسال (برای محصولات ارزان)
        /// </summary>
        public decimal? MinValue { get; set; }
        
        /// <summary>
        /// آیا کالای شکستنی را قبول می‌کند؟
        /// </summary>
        public bool AcceptsFragile { get; set; } = true;
        
        /// <summary>
        /// آیا کالای خطرناک را قبول می‌کند؟
        /// </summary>
        public bool AcceptsDangerous { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now; // تاریخ ایجاد
        public DateTime? UpdatedAt { get; set; } // تاریخ آخرین به‌روزرسانی
        
        // روابط با entity های دیگر
        public virtual ICollection<ShippingRule> Rules { get; set; } = new HashSet<ShippingRule>();
    }
}