package com.example.webservice.controller;

import com.example.webservice.dto.EmailDetail;
import com.example.webservice.dto.AuthLoginDto;
import com.example.webservice.dto.AuthRegisterDto;
import com.example.webservice.service.MailService;
import com.example.webservice.service.AuthService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequiredArgsConstructor
@RequestMapping("/auth")
@CrossOrigin
public class AuthController {
    private final MailService mailService;
    private final AuthService authService;

    @PostMapping("/register")
    public ResponseEntity<String> register(@RequestBody AuthRegisterDto authRegisterDto){
        return ResponseEntity.ok().body(authService.register(authRegisterDto));
    }

    @PostMapping("/login")
    public ResponseEntity<String> login(@RequestBody AuthLoginDto authLoginDto) throws Exception {
        return ResponseEntity.ok().body(authService.login(authLoginDto));
    }

    @GetMapping("/verify")
    public ResponseEntity<String> verifyAccount(@RequestParam("token") String token){
        return ResponseEntity.ok().body(authService.verifyAccount(token));
    }

    @GetMapping("/forget")
    public ResponseEntity<String> forgetPassword(@RequestParam("email") String email){
        return ResponseEntity.ok().body(authService.createForgetPassword(email));
    }

    @PostMapping("/sendVerifyMail")
    public ResponseEntity<String> sendVerifyEmail(@RequestBody EmailDetail emailDetail){
        return ResponseEntity.ok(mailService.sendVerifyEmail(emailDetail));
    }

    @PostMapping("/sendForgetMail")
    public ResponseEntity<String> sendForgetEmail(@RequestBody EmailDetail emailDetail){
        return ResponseEntity.ok().body(mailService.sendForgetMail(emailDetail));
    }

    @PostMapping("/sendMailWithAttachment")
    public ResponseEntity<String> sendEmailWithAttachment(@RequestBody EmailDetail emailDetailAttachment){
        return ResponseEntity.ok(mailService.sendEmailWithAttachment(emailDetailAttachment));
    }

    @GetMapping("")
    public String hello(){
        return "hello";
    }

    @GetMapping("/sendFeedback")
    public ResponseEntity<String> sendFeedback(@RequestParam("feedback") String feedback){
        return ResponseEntity.ok(mailService.sendFeedback(feedback));
    }
}
