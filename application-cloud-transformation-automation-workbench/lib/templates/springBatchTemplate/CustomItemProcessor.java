package com.hex.amaze3.engine.core;

public class CustomItemProcessor implements ItemProcessor<Transaction, Transaction> {
	 
    public Transaction process(Transaction item) {
        return item;
    }
}
