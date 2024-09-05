package com.example.webservice.service;

import com.example.webservice.dto.AuthLoginDto;
import com.example.webservice.dto.AuthRegisterDto;
import com.example.webservice.dto.UserChangePassword;

public interface AuthService {
    String login(AuthLoginDto authLoginDto) throws Exception;
    String register(AuthRegisterDto authRegisterDto);

    String verifyAccount(String token);

    String createForgetPassword(String email);
}
