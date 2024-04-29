package com.hex.amaze3.engine.core;

@SuppressWarnings("restriction")
@XmlRootElement(name = "transactionRecord")
public class Transaction {
    private String username;
    private int userId;
    private Date transactionDate;
    private double amount;
 
    /* getters and setters for the attributes */
 
    @Override
    public String toString() {
        return "Transaction [username=" + username + ", userId=" + userId
          + ", transactionDate=" + transactionDate + ", amount=" + amount
          + "]";
    }
}
