args <- commandArgs()
jewelryFilePath <- args[2]

install.packages('tseries', repos='http://cran.us.r-project.org')
install.packages('forecast', repos='http://cran.us.r-project.org')

library(tseries)
library(forecast)

#setwd("C:/Users/Luca Agatensi/Desktop/Luca/Università/Magistrale/Secondo Anno/Primo Semestre/Sistemi di Supporto alle Decisioni/Lab/R")

data <- read.csv("C:/Users/Luca Agatensi/Desktop/Luca/Università/Magistrale/Secondo Anno/Primo Semestre/Sistemi di Supporto alle Decisioni/Lab/R/esempio2.csv", header=TRUE)
#data <- read.csv(jewelryFilePath, header=TRUE)
myts <- ts(data[,3], start=c(1997,1), end=c(2008,12), frequency=12)
#plot(myts, xlab='Years', ylab = 'Sales')
#abline(reg=lm(myts~time(myts))) # linea del trend
#acf(ts(diff(myts)),main='ACF Sales')
ARIMAfit <- auto.arima(myts, stepwise = FALSE, approximation = FALSE)
summary(ARIMAfit)
#plot(ARIMAfit$x,col="black")
#lines(fitted(ARIMAfit),col="red")
NNpred=forecast(ARIMAfit, h=72)
plot(forecast(ARIMAfit, h=72))
