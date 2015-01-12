// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

#if INTERACTIVE
#r @"C:\Development\GScrape\packages\Selenium.WebDriver.2.44.0\lib\net40\WebDriver.dll"
#r @"C:\Development\GScrape\packages\Selenium.Support.2.44.0\lib\net40\WebDriver.Support.dll"
#r @"C:\Development\GScrape\packages\SizSelCsZzz.0.3.36.0\lib\SizSelCsZzz.dll"
#r @"C:\Development\GScrape\packages\Newtonsoft.Json.6.0.1-beta1\lib\net45\Newtonsoft.Json.dll"
#r @"C:\Development\GScrape\GScrape\NamelessInteractive.Insured.Infrastructure.dll"
#r @"C:\Development\GScrape\packages\canopy.0.9.18\lib\canopy.dll"
#endif

open Newtonsoft.Json
open NamelessInteractive.Insured.Infrastructure.Serializers

JsonConvert.DefaultSettings <- fun _ -> JsonSerializerSettings(Converters=[|OptionConverter(); SingleCaseDUConverter()|])

open canopy
open runner
open System
canopy.configuration.chromeDir <- @"C:\Applications\ChromeDriver"

let mutable running = false

if not running then
    running <- true
    start chrome


module Seq =
    let headOrDefault (s: 'a seq) =
        match s|> Seq.isEmpty with
        | true -> None
        | false -> s |> Seq.head |> Some

type Range =    
    {
        Min: float
        Max: float
    }

let ParseRange (text: string) =
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

let parseGemUrl urlString =
    let parseGem() =
        let gemshowtable =(element ".gemshowtable")
        let mutable rows = gemshowtable |> elementsWithin "tr"
        let getRowsAfter (afterText: string) =
            let rows = 
                rows 
                |> List.ofSeq 
                |> List.rev 
                |> Seq.takeWhile(fun r -> match (r |> someElementWithin "th.gemshowheadbar") with
                                                | None -> true
                                                | Some(e) -> 
                                                    not (e.Text.ToLower().Contains(afterText.ToLower())))
                |> List.ofSeq 
                |> List.rev
            rows
        
        let getRowContainingTextIn (text: string) (rows: OpenQA.Selenium.IWebElement seq) : OpenQA.Selenium.IWebElement option =
            let res = 
                rows |> Seq.filter (fun m -> match (m |> someElementWithin "th") with
                                                | None -> false
                                                | Some(e) -> 
                                                    e.Text.ToLower().Contains(text.ToLower()))
            res |> Seq.headOrDefault

        let getRowContainingText (text : string) : OpenQA.Selenium.IWebElement option = 
            printfn "looking for %s" text
            let res = 
                rows |> Seq.filter (fun m -> match (m |> someElementWithin "th") with
                                                | None -> false
                                                | Some(e) -> e.Text.ToLower().Contains(text.ToLower()))
            res |> Seq.headOrDefault
        let getTextFromRow (row : OpenQA.Selenium.IWebElement option) =
            match row with
            | None -> System.String.Empty
            | Some(row) -> (row |> elementWithin "td .gddataitem").Text
        let parsePhysicalProperties() = 
            
            {
                GemPhysicalProperties.MohsHardness = rows |> getRowContainingTextIn  "Moh"|> getTextFromRow |> ParseRange
                SpecificGravity = rows |> getRowContainingTextIn "Specific Gravity" |> getTextFromRow |> ParseRange
                Tenacity = rows |> getRowContainingTextIn "Tenacity" |> getTextFromRow
                CleavageQuality = rows |> getRowContainingTextIn "Cleavage Quality" |> getTextFromRow
                Fracture = rows |> getRowContainingTextIn "Fracture" |> getTextFromRow
                HeatSensitivity = rows |> getRowContainingTextIn "Heat Sensitivity" |> getTextFromRow
            }

        let parseOpticalProperties() : GemOpticalProperties =
            
            {
                GemOpticalProperties.RefractiveIndex =rows |> getRowContainingTextIn "Refractive Index" |> getTextFromRow |> ParseRange
                GemOpticalProperties.Birefringence = rows |> getRowContainingTextIn "Birefringence" |> getTextFromRow |> ParseRange
                GemOpticalProperties.Pleochroism = rows |> getRowContainingTextIn "Pleochroism" |> getTextFromRow
                GemOpticalProperties.Dispersion = rows |> getRowContainingTextIn "Dispersion" |> getTextFromRow |> ParseRange
            }

        let parseColourProperties() : GemColourProperties =
            
            {
                GemColourProperties.General = rows |> getRowContainingTextIn "Colour (General)" |> getTextFromRow
                GemColourProperties.Daylight = rows |> getRowContainingTextIn "Colour (Daylight)" |> getTextFromRow
                GemColourProperties.ChelseaFilter = rows |> getRowContainingTextIn "Colour (Chelsea Filter)" |> getTextFromRow
                GemColourProperties.CausesOfColour = rows |> getRowContainingTextIn "Causes of Colour" |> getTextFromRow
                GemColourProperties.Transparency = rows |> getRowContainingTextIn "Transparency" |> getTextFromRow
            }

        let parseFlouresenceProperties() : GemFlouresenceProperties =
            
            {
                GemFlouresenceProperties.General = rows |> getRowContainingTextIn "Fluorescence (General)" |> getTextFromRow
            }

        let parseCrystallographyProperties() : GemCrystallographyProperties =
            
            {
                GemCrystallographyProperties.CrystalSystem = rows |> getRowContainingTextIn "Crystal System" |> getTextFromRow
                GemCrystallographyProperties.Habit = rows |> getRowContainingTextIn "Habit" |> getTextFromRow
            }

        let parseGemGeologicalEnvironmentProperties() : GemGeologicalEnvironmentProperties =
            
            {
                WhereFound = rows |> getRowContainingTextIn "Where found:" |> getTextFromRow
            }

        let parseInclusionProperties() : GemInclusionProperties = 
            
            {
                Inclusions = rows |> Seq.head |> Some |> getTextFromRow 
            }

        let gemshowbox = (element ".gemshowbox")
        let name = (gemshowbox |> elementWithin "h1").Text
        let description = (gemshowbox |> elementWithin "#gemintro").Text
        rows <- getRowsAfter "Physical Properties of Amethyst"
        let physical = parsePhysicalProperties()
        rows <-  getRowsAfter "Optical Properties of Amethyst"
        let optical = parseOpticalProperties()
        rows <- getRowsAfter "Colour"
        let colour = parseColourProperties()
        rows <- getRowsAfter "Fluorescence & other light emissions"
        let flouresence = parseFlouresenceProperties()
        rows <- getRowsAfter "Crystallography of Amethyst"
        let crystallography = parseCrystallographyProperties()
        rows <- getRowsAfter "Geological Environment"
        let geological = parseGemGeologicalEnvironmentProperties()
        rows <- getRowsAfter "Inclusions in Amethyst"
        let inclusions = parseInclusionProperties()
        {
            Gem.Name = name
            Gem.Description = description
            Gem.PhysicalProperties = physical
            Gem.OpticalProperties = optical
            Gem.ColourProperties = colour
            Gem.FlouresenceProperties = flouresence
            Gem.Crystallography = crystallography
            Gem.GeologicalEnvironment = geological
            Gem.Inclusions = inclusions
        }    
    let curUrl = currentUrl()
    if curUrl <> urlString then
        url urlString
    parseGem()
    

let getAndStoreGem(url: string) =
    printfn "Parsing url %s" url
    try
        let gem = parseGemUrl url
        let json = JsonConvert.SerializeObject(gem, Formatting.Indented)
        System.IO.File.WriteAllText(@"C:\Development\GScrape\GScrape\Gems\" + gem.Name + ".txt", json)
    with
    | ex -> printfn "Error parsing url %s - %s" url ex.Message

let json = JsonConvert.SerializeObject(DateTime.Now, Formatting.Indented)

url "http://www.gemdat.org/gemindex.php"
let links = elements ".fancybox li > a" |> Seq.map (fun e -> e.GetAttribute("href")) |> List.ofSeq
links |> Seq.iter getAndStoreGem
printfn "All done"
System.Console.ReadLine()