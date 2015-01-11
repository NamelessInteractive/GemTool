namespace NamelessInteractive.GemTool.Models

type Range =    
    {
        Min: float
        Max: float
    }
    static member Parse (text: string) : Range option =
        if (text.ToLower().Contains("to")) then
            let toIndex = text.ToLower().IndexOf("to")
            let minText = text.Substring(0,toIndex).Trim()
            let maxText = text.Substring(toIndex + 2).Trim()
            try
                {
                    Min = minText |> float
                    Max = maxText |> float
                } |> Some
            with 
            | ex -> None
        else
            try
                let value = text |> float
                {
                    Min = value
                    Max = value
                } |> Some
            with
            | ex -> None

type GemPhysicalProperties =
    {
        MohsHardness: Range option
        SpecificGravity: Range option
        Tenacity: string
        CleavageQuality: string
        Fracture: string
        HeatSensitivity: string
    }

type GemOpticalProperties =
    {
        RefractiveIndex: Range option
        Birefringence: Range option
        Pleochroism: string
        Dispersion: Range option
    }

type GemColourProperties =
    {
        General: string
        Daylight: string
        ChelseaFilter: string
        CausesOfColour: string
        Transparency: string
    }

type GemFlouresenceProperties =
    {
        General: string
    }

type GemCrystallographyProperties =
    {
        CrystalSystem: string
        Habit: string
    }

type GemGeologicalEnvironmentProperties =
    {
        WhereFound: string
    }

type GemInclusionProperties =
    {
        Inclusions: string
    }

type Gem =
    {
        Name: string
        Description: string
        PhysicalProperties: GemPhysicalProperties
        OpticalProperties: GemOpticalProperties
        ColourProperties: GemColourProperties
        FlouresenceProperties: GemFlouresenceProperties
        Crystallography: GemCrystallographyProperties
        GeologicalEnvironment: GemGeologicalEnvironmentProperties
        Inclusions: GemInclusionProperties
    }