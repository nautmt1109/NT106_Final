package com.example.webservice.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class EmailDetail {

    private String recipient;
    private String subject;
    private String message;
    private String attachment;
}
