open System
open FSharp.Data

// let format = System.Globalization.CultureInfo.CreateSpecificCulture("hr-HR")
type CurrResponse = JsonProvider<"https://api.hnb.hr/tecajn/v2", Culture="hr-HR">


// prodajniTecaj is Croatian for quote when selling currency to bank
let currencies = CurrResponse.GetSamples() 
                |> Seq.map(fun x -> (x.Valuta, double x.ProdajniTecaj)) |> Map

type Currency = Currency of string

type IExchangeRates =
    abstract member GetRate : fromCurrency: string -> toCurrency: string -> unit

type Cash = 
    { Quantity: double
      Currency: Currency }

    member this.Value() = this.Quantity

type Stock =
    { Symbol: string
      Shares: double
      Price: double 
      Currency: Currency}

    member this.Value() = this.Shares * this.Price

type Asset =
    | Cash of Cash
    | Stock of Stock

    member this.Value =
        match this with
        | Cash c -> (c.Value(), c.Currency)
        | Stock s -> (s.Value(), s.Currency)

type AssetPortfolio() =
    let portfolio = ResizeArray<Asset>()

    member this.Add(a) = portfolio.Add(a)

    member this.Value currency =
        let mutable v = 0.0

        for asset in portfolio do
            match asset.Value with 
            |  (x,y) when y = currency -> v <- x
            | (x,y) when y <> currency 
        v

    member this.Consolidate() : AssetPortfolio = failwith "not yet implemented"

let AreEqual (a: double, b: double) = Math.Abs(a - b) < 0.0001



[<EntryPoint>]
let main argv =
    let assetPortfolio = AssetPortfolio()

    assetPortfolio.Add( Stock
        { Symbol = "ABC"
          Shares = 200.0
          Price = 4.0 
          Currency = Currency "GBP"}
    )

    assetPortfolio.Add( Stock
        { Symbol = "DDW"
          Shares = 100.0
          Price = 10.0 
          Currency = Currency "GBP"}
    )

    assetPortfolio.Add(Cash {
        Quantity = 100.0
        Currency = Currency "USD"
    })

    if not <| AreEqual(assetPortfolio.Value(), 1800.0) then
        printfn "Test1 Failed, Expected Value: %f, Actual Value: %f" 1800.0 (assetPortfolio.Value())

    printfn "%A" currencies

    printfn "Done... (Press a key to close)"
    Console.ReadKey() |> ignore
    0
