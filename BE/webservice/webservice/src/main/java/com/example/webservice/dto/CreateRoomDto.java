package com.example.webservice.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class CreateRoomDto {
    private String username;
    private String roomId;
    private int typeMoney;
    private int numberPeople;
}
