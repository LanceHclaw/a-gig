using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class AllEvidence
{
    public static readonly Evidence MagnumShell = new Evidence(nameof(MagnumShell));
    public static readonly Evidence Letter = new Evidence(nameof(Letter));
    public static readonly Evidence Pillow = new Evidence(nameof(Pillow));
    public static readonly Evidence GunInBox = new Evidence(nameof(GunInBox));
    public static readonly Evidence Cigarette = new Evidence(nameof(Cigarette));
    public static readonly Evidence FloorMop = new Evidence(nameof(FloorMop));
    public static readonly Evidence CleanFloor = new Evidence(nameof(CleanFloor));
    public static readonly Evidence OpenWindow = new Evidence(nameof(OpenWindow));
    public static readonly Evidence Dummy = new Evidence(nameof(Dummy));
    public static readonly Evidence PackOfCigarettes = new Evidence(nameof(PackOfCigarettes));
    public static readonly Evidence BulletInBody = new Evidence(nameof(BulletInBody));
    public static readonly Evidence Journal = new Evidence(nameof(Journal));
    public static readonly Evidence Knife = new Evidence(nameof(Knife));

    public static IEnumerable<Evidence> GetAllEvidence()
    {
        return new List<Evidence> { MagnumShell, Letter, Pillow, GunInBox, Cigarette, FloorMop, CleanFloor,
            OpenWindow, Dummy, PackOfCigarettes, BulletInBody, Journal, Knife };
    }
}

