# THE PROBLEM
A portfolio can have stocks and currency assets (Cash in a particular
currency, normally acquired through Forex transactions). We want to
be able to track the assets, the average purchase cost and value the
portfolio using current exchange rates.

The code currently handles only domestic stocks

# THE CHALLENGE
1. Modify the design to accommodate multiple asset types:
   1. currency assets (i.e. 1000 EUR)
   2. currency denominated stocks (i.e. stock in GBP)
   3. the design should be extendable to accommodate new asset types in the future

2. Adjust the design to allow valuing the portfolio in any
   currency (i.e. given current exchange rates, be able to find
   the value of the entire portfolio, for example, in USD, GBP,
   EUR, etc.)
3. Complete the function to consolidate the portfolio by unique 
   asset and average cost.

   For example if the portfolio consists of the following assets:
   1. 100 shares of ABC stock at $2 USD
   2. 200 shares of ABC stock at $3.50 USD
   3. Cash of 1000 EUR
   4. Cash of 200 EUR

   The consolidated portfolio will have two assets:
   1. 300 shares of ABC stock at $3 USD
   2. Cash of 1200 EUR

[For simplicity, we assume the stock market is at the same price we
bought at, and value is based on cost]
Feel free to make any changes to the initial code, use any libraries
or add additional projects.