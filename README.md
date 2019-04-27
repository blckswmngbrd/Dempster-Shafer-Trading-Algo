# Dempster-Shafer-Trading-Algo

DST Market Mind - "High Frequency" style trading algo based on the Dempster-Shafer fusion theory in C# using
the Interactive Brokers API.  

Dempster-Shafer Theory (DST): data fusion method based on mass functions
(Code Sample)

• Event based automated trading strategy

• Use's Dempster’s rule of combination to fuse information from multiple independent
  sources
  
• User/Subject Matter Experts select parameters used to generate mass functions.


• Mass function 1: market microstructure(spread, order book depth, queue, short interest, etc.)
  
  Elements of Market Depth evaluated:
  Bid Ask Depth and Thickness
  Spread > Average Spread
  Bid Price Improvement  Degree in which the Bid price improves 
  Ask Price Improvement  Degree in which the Ask price improves 
  Ask Size  Improvement  vs. Average Ask Size Improvement
  Bid Size  Improvement  vs. Bid Ask Size Improvement
  
• Mass function 2: Event based Moving Average (EMA) shrinkage and crossovers

• Assigns weights solely on what information is available. Not hindered by missing information.

• Utilize's Interactive Brokers TWS C# API 

• Fully functional GUI

• Estimated refresh rate: 5-15 seconds

 
Developed
by 

Randall Campbell and Francisco Hernandez
@ The Illinois Institute of Technology - Stuart School of Business
