args <- commandArgs()
exampleFilePath <- args[2]

#install.packages('nnet')
#install.packages('forecast')

library(nnet)
library(forecast)
list <- list(1,2,3,4,5)
#setwd("C:/Users/Luca Agatensi/Desktop/Luca/Università/Magistrale/Secondo Anno/Primo Semestre/Sistemi di Supporto alle Decisioni/Lab/R")

dati <-read.csv(exampleFilePath, header=TRUE)
myts <- ts(dati[,3], start=c(2004,1), end=c(2008,4), frequency=4)
myts2 <- window(myts, start=c(2004,1), end=c(2007,4))
NNfit <- nnetar(myts2)
summary(NNfit$model[[1]])
NNpred = forecast(NNfit,h=4)
plot(NNpred)
summary(NNpred)
