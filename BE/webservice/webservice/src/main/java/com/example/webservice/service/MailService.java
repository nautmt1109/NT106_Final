package com.example.webservice.service;

import com.example.webservice.dto.EmailDetail;

public interface MailService {
    String sendEmailWithAttachment(EmailDetail emailDetailAttachment);
    String sendVerifyEmail(EmailDetail emailDetail);

    String sendForgetMail(EmailDetail emailDetail);

    String sendFeedback(String email);
}
