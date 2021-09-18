open System

type Cash = { Quantity: double; Currency: string }

type Stock =
    { Symbol: string
      Shares: double
      Price: double }

    member this.Value() = this.Shares * this.Price

type Asset =
    | Cash of Cash
    | Stock of Stock

type AssetPortfolio() =
    let portfolio = ResizeArray<Stock>()


    member this.Add(a) = portfolio.Add(a)

    member this.Value() =
        let mutable v = 0.0

        for stock in portfolio do
            v <- v + stock.Value()
        v

    member this.Consolidate() : AssetPortfolio = failwith "not yet implemented"

let AreEqual (a: double, b: double) = Math.Abs(a - b) < 0.0001

type IExchangeRates =
    abstract member GetRate : fromCurrency: string -> toCurrency: string -> unit

[<EntryPoint>]
let main argv =
    let assetPortfolio = AssetPortfolio()

    assetPortfolio.Add(
        { Symbol = "ABC"
          Shares = 200.0
          Price = 4.0 }
    )

    assetPortfolio.Add(
        { Symbol = "DDW"
          Shares = 100.0
          Price = 10.0 }
    )

    if not <| AreEqual(assetPortfolio.Value(), 1800.0) then
        printfn "Test1 Failed, Expected Value: %f, Actual Value: %f" 1800.0 (assetPortfolio.Value())

    printfn "Done... (Press a key to close)"
    Console.ReadKey() |> ignore
    0
