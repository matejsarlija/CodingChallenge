open System
open FSharp.Data

// let format = System.Globalization.CultureInfo.CreateSpecificCulture("hr-HR")
type CurrResponse = JsonProvider<"https://api.hnb.hr/tecajn/v2", Culture="hr-HR">

type Currency = Currency of string

// "prodajniTecaj" is Croatian for quote when selling currency to bank
let currencies = CurrResponse.GetSamples() 
                |> Seq.map(fun x -> (Currency x.Valuta, double x.ProdajniTecaj)) 
                |> Map

type IExchangeRates =
    abstract member GetRate : fromCurrency: Currency -> toCurrency: Currency -> double

type Cash = 
    { Quantity: double
      Currency: Currency }

    member this.Value() = this.Quantity

type Stock =
    { Symb: string
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

    let cashArr = ResizeArray<Cash>()
    let stockArr = ResizeArray<Stock>()

    member this.Add(a) = 
        match a with 
        | Cash a ->  cashArr.Add(a)
        | Stock a -> stockArr.Add(a)

        portfolio.Add(a)

    interface IExchangeRates with
        member this.GetRate lhs rhs =
            let x = if lhs = Currency "HRK" then 1.0 else Map.find lhs currencies
            let y = Map.find rhs currencies
            y / x
        
    member this.Value currency =
        let mutable v:double = 0.0

        for asset in portfolio do
            match asset.Value with 
            | (x,y) when y = currency -> v <- v + x
            | (x,y) when y <> currency -> v <- v + x * (this :> IExchangeRates).GetRate currency y
            | _ -> printf "Something's wrong with the portfolio lookup at the moment." 
        v

    // : AssetPortfolio
    member this.Consolidate() =
        let ourCashResult = 
            cashArr
            |> Seq.groupBy(fun x -> x.Currency)
            |> Seq.map (fun (x, y) -> (x , y |> Seq.sumBy (fun x -> x.Quantity)))

        let ourStockResult = 
            stockArr
            |> Seq.groupBy(fun x -> x.Symb, x.Currency)
            |> Seq.map (fun (x, y) -> (x, y |> Seq.sumBy (fun x -> x.Shares), y |> Seq.averageBy (fun x -> x.Price)))

        printfn "%A" ourCashResult
        printfn "%A" ourStockResult

let AreEqual (a: double, b: double) = Math.Abs(a - b) < 0.0001

[<EntryPoint>]
let main argv =
    let assetPortfolio = AssetPortfolio()

    assetPortfolio.Add( Stock
        { Symb = "ABC"
          Shares = 200.0
          Price = 4.0 
          Currency = Currency "GBP"}
    )

    assetPortfolio.Add( Stock
        { Symb = "DDW"
          Shares = 100.0
          Price = 10.0 
          Currency = Currency "GBP"}
    )

    assetPortfolio.Add(Cash {
        Quantity = 100.0
        Currency = Currency "USD"
    })

    assetPortfolio.Add(Cash {
        Quantity = 200.0
        Currency = Currency "GBP"
    })

    assetPortfolio.Add(Cash {
        Quantity = 100.0
        Currency = Currency "USD"
    })

    assetPortfolio.Add( Stock
        { Symb = "DDW"
          Shares = 200.0
          Price = 9.0 
          Currency = Currency "USD"}
    )

    assetPortfolio.Add( Stock
        { Symb = "MRN"
          Shares = 150.0
          Price = 120.0 
          Currency = Currency "GBP"}
    )

    if not <| AreEqual(assetPortfolio.Value(Currency "USD"), 1800.0) then
        printfn "Test1 Failed, Expected Value: %f, Actual Value: %f" 1800.0 (assetPortfolio.Value(Currency "HRK"))

    assetPortfolio.Consolidate()

    printfn "Done... (Press a key to close)"
    Console.ReadKey() |> ignore
    0
