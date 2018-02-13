#install and load necessary packages 
#install.packages(c("quantmod","TTR","highfrequency"))
library(highfrequency)
library(quantmod)
library(TTR)

#Download sample data
getSymbols.yahoo("BK",env=globalenv(),from="2015-03-01",to="2017-03-06")


#Isolate Closing Prices
ClosePrices<-na.omit(BK$BK.Close)

#Reverse price order
ClosePricesRev<-as.matrix(ClosePrices)[nrow(ClosePrices):1,,drop=FALSE]
ClosePricesRev<-na.omit(ClosePricesRev)

#create simple averages 
library(TTR)
simple10<-SMA(ClosePricesRev,n=10,price=TRUE)
simple20<-SMA(ClosePricesRev,n=20,price=TRUE)
simple30<-SMA(ClosePricesRev,n=30,price=TRUE)
simple50<-SMA(ClosePricesRev,n=50,price=TRUE)

#create exponential moving averages
exp10<-EMA(ClosePricesRev,n=10,prices=TRUE)
exp20<-EMA(ClosePricesRev,n=20,prices=TRUE)
exp30<-EMA(ClosePricesRev,n=30,prices=TRUE)
exp50<-EMA(ClosePricesRev,n=50,prices=TRUE)

#Find the number of S/EMA crossovers everyday 
#first component is bull crossover
#second component is bear crossover

#crossovers<-matrix(length(exp50)-49,3)
#crossovers[,3]<-50:252

crossovers<-matrix(0L,nrow=length(exp50)-49,ncol=3)

for (i in 51:length(exp50)){
    
    #check for crossover types (if any) in 10-20 EMAs
    cross_10_20_prev <- sign(exp10[i-1] - exp20[i-1])
    cross_10_20_now  <- sign(exp10[i] - exp20[i])
    
    #bull crossover
    if(cross_10_20_prev < cross_10_20_now){
        crossovers[i-49,1] <- crossovers[i-49,1]+ 1   
    }
    
    #bear crossover
    else if (cross_10_20_prev > cross_10_20_now){
        crossovers[i-49,2] <- crossovers[i-49,2] + 1 
    }
    
    #check for crossover types in 10-50 EMAs
    cross_10_50_prev <- sign(exp10[i-1] - exp50[i-1])
    cross_10_50_now <- sign(exp10[i] - exp50[i])
    
    if(cross_10_50_prev < cross_10_50_now){
        crossovers[i-49,1] <- crossovers[i-49,1] + 1
    }
    
    else if (cross_10_50_prev > cross_10_50_now){
        crossovers[i-49,2] <- crossovers[i-49,2] + 1
    }
    
    #check for crossover types in the 20-50 EMAs
    cross_20_50_prev <- sign(exp20[i-1] - exp50[i-1])
    cross_20_50_now <- sign(exp20[i] - exp50[i])
    
    if(cross_20_50_prev < cross_20_50_now){
        crossovers[i-49,1] <- crossovers[i-49,1] + 1
    } 
    else if(cross_20_50_prev > cross_20_50_now){
        crossovers[i-49,2] <- crossovers[i-49,2] + 1
    }
    
}


if(crossovers[nrow(crossovers),1] == 0 && crossovers[nrow(crossovers),2] == 0){
    
    diff_now_10_20 <- (exp10[nrow(exp10),ncol(exp10)]-exp20[nrow(exp20),ncol(exp20)])      #current time step diff
    diff_prev_10_20 <-(exp10[nrow(exp10)-1,ncol(exp10)]-exp20[nrow(exp20)-1,ncol(exp20)])  #previous time step diff
    diff_now_10_50 <- (exp10[nrow(exp10),ncol(exp10)]-exp50[nrow(exp50),ncol(exp50)])      #current time step diff
    diff_prev_10_50 <-(exp10[nrow(exp10)-1,ncol(exp10)]-exp50[nrow(exp50)-1,ncol(exp50)])  #previous time step diff
    diff_now_20_50 <- (exp20[nrow(exp20),ncol(exp20)]-exp50[nrow(exp50),ncol(exp50)])      #current time step diff
    diff_prev_20_50 <-(exp20[nrow(exp20)-1,ncol(exp20)]-exp50[nrow(exp50)-1,ncol(exp50)])
    
    num_shrinks <- (diff_now_10_20 < diff_prev_10_20) + (diff_now_10_50 < diff_prev_10_50) + (diff_now_20_50 < diff_prev_20_50)
    
    #print(num_shrinks)
    
    if(diff_now_10_20 > 0){   
        if(num_shrinks == 3){
            param_m1 <- 1
        }
        else if(num_shrinks == 2){
            param_m1 <- 2
        }
        
        else if(num_shrinks == 1){
            param_m1 <-3
        }
        else {
            param_m1 <-4 
        }
        
    }
    else if(diff_now_10_20 < 0){ #if already bearish, now we basically have the same as the previous case, but flipped
        
    }else if(num_shrinks == 2){
        param_m1 <- 2
    }else if(num_shrinks == 1){
        param_m1 <- 2
    } else {
        param_m1<-1
    }        
    
    #Case 2: one bullish crossover, and 2 bearish 
    #for these following cases starting from here, we will not look at the
    #differences at all, just the number of crossovers that we have. We will
    #consider any crossover (10-20, 20-50, 10-50) to be equivalent in terms of
    #our distribution. 
    
    if(crossovers[nrow(crossovers),1] == 1 && crossovers(nrow[crossovers],2) == 0){ #one bullish
        param_m1 <- 1
    }
    
    else if(crossovers[nrow(crossovers),1] == 2 && crossovers[nrow(crossovers),2] == 0){ #two bullish
        param_m1 <- 4
    }
    else if(crossovers[nrow(crossovers),1] == 3 && crossovers[nrow(crossovers),2] == 0){  #three bullish (this is as good as it gets)
        param_m1 <- 4
    }   
    else if(crossovers[nrow(crossovers),1] == 1 && crossovers[nrow(crossovers),2] == 1){ #one bullish, one bearish
        param_m1 <- 5
    }
    else if(crossovers[nrow(crossovers),1] == 2 && crossovers[nrow(crossovers),2] == 1){#two bullish, one bearish
        param_m1 <- 3
    }
    else if(crossovers[nrow(crossovers),1] == 1 && crossovers[nrow(crossovers),2] == 2){#one bullish, two bearish
        param_m1 <- 2
    }
    else if(crossovers[nrow(crossovers),1] == 0 && crossovers[nrow(crossovers),2] == 1){ #one bearish
        param_m1 <-2
    }
    else if(crossovers[nrow(crossovers),1] == 0 && crossovers[nrow(crossovers),2] == 2){ #two bearish
        param_m1 <-1
    }
    else if(crossovers[nrow(crossovers),1] == 0 && crossovers[nrow(crossovers),3] == 3){ #three bearish (this is the worst possible case)
        param_m1 <- 3   
    }
    
} 

m1<-c()
m1[0]<-0
x<-seq(-10,10,by=.10)

for(i in 1:11){integrand<-function(x){dlogis(x,2,param_m1)}

m1[i]  <- integrate(integrand,lower = 2*i -13, upper = 2*i - 11)
m1[12] <- 1- do.call(sum,m1[1:11]) 
}

library(highfrequency)
data(sample_qdata)

#Convert sample Bid and Ask data to dataframe
sqdatadf<-as.data.frame(sample_qdata)

#Convert Bid and ASK size columns to numerics
changebidsiznum<-as.numeric(sqdatadf$BIDSIZ)
changeofrsiznum<-as.numeric(sqdatadf$OFRSIZ)
Bidnum<-as.numeric(sqdatadf$BID)
#OFRnum<-as.numeric(sqdatadf$OFR)

#Create data set of the changes in the size of the Bid ASK size
#ChangeOFRsize<-diff(changeofrsiznum,lag=1)
ChangeBIDsize<-diff(changebidsiznum,lag=1)

#Create data set for the changes in the Bid Ask price 
ChangeBID<-diff(log(Bidnum),lag=1)
#ChangeOFR<-diff(log(OFRnum),lag=1)

#using library scales
#library("scales")

#rescale data to -10 to 10
#>reChangeBID<-rescale_mid(ChangeBID,to=c(-10,10),mid= 0)
#>reChangeOFR<-rescale_mid(ChangeOFR,to=c(-10,10),mid= 0)
#>reChangeBIDsize<-rescale_mid(ChangeBIDsize,to=c(-10,10),mid= 0)
#>reChangeOFRsize<-rescale_mid(ChangeOFRsize,to=c(-10,10),mid= 0)


#require(GLDEX)

#Plot of the Generalized Lambda Distribution for each 
#ChangeBID/ChangeBIDSIZE/ChangeASK/ChangeASKSIZE

##> reChangeBIDsizeLambda<- fun.RMFMKL.lm(reChangeBIDsize)
##> fun.theo.mv.gld(reChangeBIDsizeLambda[1],reChangeBIDsizeLambda[2],reChangeBIDsizeLambda[3],reChangeBIDsizeLambda[4], param="fmkl",normalise = "N")
##> bins_2<-nclass.FD(reChangeBIDsize)
##> fun.plot.fit(fit.obj = reChangeBIDsizeLambda,data=reChangeBIDsize,nclass=bins_2,param.vec = "fmkl")
##> reChangeBIDsizeLambdaPlot<-fun.plot.fit(fit.obj = reChangeBIDsizeLambda,data=reChangeBIDsize,nclass=bins_2,param.vec = "fmkl")

##> reChangeOFRsizeLambda<- fun.RMFMKL.lm(reChangeOFRsize)
##> fun.theo.mv.gld(reChangeOFRsizeLambda[1],reChangeOFRsizeLambda[2],reChangeOFRsizeLambda[3],reChangeOFRsizeLambda[4], param="fmkl",normalise = "N") 
##> bins_3<-nclass.FD(reChangeOFRsize)
##> reChangeOFRsizeLambdaPlot<-fun.plot.fit(fit.obj = reChangeOFRsizeLambda,data=reChangeOFRsize,nclass=bins_3,param.vec = "fmkl")

##> reChangeOFRLambda<-fun.RMFMKL.lm(reChangeOFR)
##> fun.theo.mv.gld(reChangeOFRLambda[1],reChangeOFRLambda[2],reChangeOFRLambda[3],reChangeOFRLambda[4], param="fmkl",normalise = "N")
##> bins_4<-nclass.FD(reChangeOFR)
##> reChangeOFRLambdaPlot<-fun.plot.fit(fit.obj = reChangeOFRLambda,data=reChangeOFR,nclass=bins_4,param.vec = "fmkl")

##> reChangeBIDLambda<-fun.RMFMKL.lm(reChangeBID)
##> fun.theo.mv.gld(reChangeBIDLambda[1],reChangeBIDLambda[2],reChangeBIDLambda[3],reChangeBIDLambda[4], param="fmkl",normalise = "N")
##> bins_5<-nclass.FD(reChangeBID)
##> reChangeBIDLambdaPlot<-fun.plot.fit(fit.obj = reChangeBIDLambda,data=reChangeBID,nclass=bins_5,param.vec = "fmkl")

#Reformat to matrices
ChangeBID<-as.matrix(ChangeBID)

ChangeBIDsize<-as.matrix(ChangeBIDsize)

#reChangeOFR<-as.matrix(reChangeOFR)

#reChangeOFRsize<-as.matrix(reChangeOFRsize)

#An increase(+) in reChangeBidSize and reChangeBID is Strong Bullish indicator
#A decrease(-) in reChangeBidSize is a Bearish indicator
#An increase(+) in reChangeBidSize is a Bullish indicator
#A decrease(-) in reChangeOFR is a Bearish indicator
#An increase(+) in reChangeOFRsize is a Bearish indicator

BidMove<-matrix(0L,nrow=length(ChangeBID)-7703,ncol=3)

BidSizeMove<-matrix(0L,nrow=length(ChangeBIDsize)-7701,ncol=3)


for (i in 7700:length(ChangeBID)){
    
    #Check for increases in Bid price
    BidMove_prev <- sign(ChangeBID[i-4])
    BidMove_now <-sign(ChangeBID[i])
    
    #Bullish - Joins to the Bid
    if(BidMove_prev < BidMove_now){
        BidMove[i-7703,1] <- BidMove[i-7703,1]+1
        
    }
    
    #Bearish - No Joins to the Bid
    else if (BidMove_prev > BidMove_now){
        BidMove[i-7703,2] <- BidMove[i-7703,2]+1 
        
    }
    
    #Check for Bid size Movement
    BidSizeMove_prev <- sign(ChangeBIDsize[i-1])
    BidSizeMove_now <-  sign(ChangeBIDsize[i])
    
    #Bullish - Joins to the Bid
    if(BidSizeMove_prev < BidSizeMove_now){          
        BidSizeMove[i-7701,1] <- BidSizeMove[i-7701,1]+1 
        
    }
    
    #Bearish - No Joins to the Bid
    else if (BidSizeMove_prev > BidSizeMove_now){
        BidSizeMove[i-7701,2] <- BidSizeMove[i-7701,2]+1 
        
    }
    
}


if(BidMove[nrow(BidMove),1] == 0 && BidMove[nrow(BidMove),2] == 0){
    
    param_m2 <- 1
    
    if(BidMove[nrow(BidMove),1] == 1 && BidMove(nrow(BidMove),2) == 0){ #one bullish
        param_m2 <- 2
    }
    
    else if(BidMove[nrow(BidMove),1] == 2 && BidMove[nrow(BidMove),2] == 0){ #two bullish
        param_m2 <- 3
    }
    
    else if(BidMove[nrow(BidMove),1] == 3 && BidMove[nrow(BidMove),2] == 0){  #three bullish (this is as good as it gets)
        param_m2 <- 4
    }   
    
    else if(BidMove[nrow(BidMove),1] == 1 && BidMove[nrow(BidMove),2] == 1){ #one bullish, one bearish
        param_m2 <- 2
    }
    
    else if(BidMove[nrow(BidMove),1] == 2 && BidMove[nrow(BidMove),2] == 1){#two bullish, one bearish
        param_m2 <- 2
    }
    
    else if(BidMove[nrow(BidMove),1] == 1 && BidMove[nrow(BidMove),2] == 2){#one bullish, two bearish
        param_m2 <-5
    }
    
    else if(BidMove[nrow(BidMove),1] == 0 && BidMove[nrow(BidMove),2] == 1){ #one bearish
        param_m2 <-1
    }
    
    else if(BidMove[nrow(BidMove),1] == 0 && BidMove[nrow(BidMove),2] == 2){ #two bearish
        param_m2 <-3
    }
    
    else if(BidMove[nrow(BidMove),1] == 0 && BidMove[nrow(BidMove),3] == 3){ #three bearish (this is the worst possible case)
        param_m2 <-1    
    }
}       

#Apply mass functions to necessary Bid changes


m2<-c()
m2[0]<-0

for(i in 1:11){
    integrand<-function(x){dlogis(x,1,param_m2)}
    m2[i]  <- integrate(integrand,lower = 2*i -13, upper = 2*i - 11)
    m2[12] <- 1- do.call(sum,m2[1:11])
}

#m1<-m1[1:11]
#m2<-m2[1:11]

#DSTfusion<-function(m1,m2){
kappa = 0
for(i in 1:11){
    #m1[i]<-as.numeric(m1[i])
    #m2[i]<-as.numeric(m2[i])
    
    for(j in 1:11){
        if(i != j){
            kappa = kappa + as.numeric(unlist(m1[i]))* as.numeric(unlist(m2[j]))
        }
    }
}

K = 1/(1-kappa)

# this will compute the fused masses for all the events
M <-as.numeric()

for(i in 1:11){ 
    M = as.numeric(M,K*(as.numeric(unlist(m1[i]))*as.numeric(unlist(m2[i])) + as.numeric(unlist(m1[i]))*as.numeric(unlist(m2[12])) + as.numeric(unlist(m1[12]))*as.numeric(unlist(m2[i]))))
    }
    
    MI = c(M,K*(as.numeric(unlist(m1[12]))*as.numeric(unlist(m2[12]))))
    
    M=1-MI

    cat("The mass function is",M,".This is a BULLISH indicator, that you should add to you your current position or BUY at the market.")