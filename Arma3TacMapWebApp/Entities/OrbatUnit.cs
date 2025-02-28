using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pmad.Milsymbol.App6d;
using Pmad.Milsymbol.AspNetCore.Orbat;

namespace Arma3TacMapWebApp.Entities
{
    public class OrbatUnit : IOrbatUnit
    {
        public int OrbatUnitID { get; set; }

        public int OrbatID { get; set; }
        public Orbat? Orbat { get; set; }

        [Display(Name = "Parent Unit")]
        public int? ParentOrbatUnitID { get; set; }
        [Display(Name = "Parent Unit")]
        public OrbatUnit? Parent { get; set; }

        [Display(Name = "Call sign")]
        public string? Name { get; set; }

        [Display(Name = "Unique Designation")]
        public string? UniqueDesignation { get; set; }

        [Display(Name = "Symbol set")]
        public string? NatoSymbolSet { get; set; }

        [Display(Name = "Symbol")]
        public string? NatoSymbolIcon { get; set; }

        [Display(Name = "Icon Modifier 1")]
        public string? NatoSymbolMod1 { get; set; }

        [Display(Name = "Icon Modifier 2")]
        public string? NatoSymbolMod2 { get; set; }

        [Display(Name = "Echelon")]
        public string? NatoSymbolSize { get; set; }

        [Display(Name = "HQ, TF")]
        public string? NatoSymbolHQ { get; set; }

        [Display(Name = "Friendly Symbol")]
        public string? NatoSymbolFriendlyImageBase64 { get; set; }
        
        [Display(Name = "Hostile Symbol")]
        public string? NatoSymbolHostileImageBase64 { get; set; }

        [Display(Name = "Hostile Symbol")]
        public string? NatoSymbolHostileAssumedImageBase64 { get; set; }

        public int Position { get; set; }

        public string GetNatoSymbol(char c, char e = '0') => "100" + c + (NatoSymbolSet ?? "10") + e +
            (NatoSymbolHQ ?? string.Empty).PadLeft(1, '0') +
            (NatoSymbolSize ?? string.Empty).PadLeft(2, '0') +
            (NatoSymbolIcon ?? string.Empty).PadLeft(6, '0') +
            (NatoSymbolMod1 ?? string.Empty).PadLeft(2, '0') +
            (NatoSymbolMod2 ?? string.Empty).PadLeft(2, '0');

        public List<OrbatUnit>? Children { get; set; }

        [NotMapped]
        public int RelativeLevel { get; set; }

        [Display(Name = "Trigram")]
        [MaxLength(3)]
        public string? Trigram { get; set; }

        [NotMapped]
        public string FriendSidc
        {
            get { return GetNatoSymbol('3'); }
            set
            {
                if (App6dSymbolId.TryParse(value, out var symbolId))
                {
                    NatoSymbolHQ = symbolId.HqTfFdCode;
                    NatoSymbolSize = symbolId.Amplifier;
                    NatoSymbolIcon = symbolId.Icon;
                    NatoSymbolMod1 = symbolId.Modifier1;
                    NatoSymbolMod2 = symbolId.Modifier2;
                    NatoSymbolSet = symbolId.SymbolSet;
                }
                else
                {
                    NatoSymbolHQ = "0";
                    NatoSymbolSize = "00";
                    NatoSymbolIcon = "000000";
                    NatoSymbolMod1 = "00";
                    NatoSymbolMod2 = "00";
                    NatoSymbolSet = "10";
                }
            }
        }

        string IOrbatUnit.Sdic => GetNatoSymbol('3');

        string? IOrbatUnit.CommonIdentifier => Name;

        string? IOrbatUnit.HigherFormation => string.Empty;

        string? IOrbatUnit.UniqueDesignation => string.Empty;

        IEnumerable<IOrbatUnit>? IOrbatUnit.SubUnits => Children;

        internal bool IsSelfOrParent(OrbatUnit orbatUnit)
        {
            if (orbatUnit.OrbatUnitID == OrbatUnitID || orbatUnit.OrbatUnitID == ParentOrbatUnitID)
            {
                return true;
            }
            if (Parent != null)
            {
                return Parent.IsSelfOrParent(orbatUnit);
            }
            return false;
        }
    }
}
