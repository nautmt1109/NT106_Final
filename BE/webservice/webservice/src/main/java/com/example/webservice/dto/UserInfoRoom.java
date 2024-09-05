package com.example.webservice.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class UserInfoRoom {
    private String user1;
    private String user2;
    private double money1;
    private double money2;
}
