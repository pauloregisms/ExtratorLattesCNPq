//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LattesExtractor.Entities.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class PeriodicoQualis
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PeriodicoQualis()
        {
            this.ExtratoQualis = new HashSet<ExtratoQualis>();
            this.ProducaoBibliografica = new HashSet<ProducaoBibliografica>();
        }
    
        public string ISSNPeriodicoQualis { get; set; }
        public string TituloPeriodicoQualis { get; set; }
        public int PeriodicoQualisId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExtratoQualis> ExtratoQualis { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProducaoBibliografica> ProducaoBibliografica { get; set; }
    }
}