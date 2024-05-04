using System.ComponentModel.DataAnnotations;

namespace SDK.Shared.Entities
{
    public abstract class BaseEntity<T>
    {
        [Key]
        public virtual T Rowid { get; set; } = default!;
    }
}
