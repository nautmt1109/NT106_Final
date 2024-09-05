package com.example.webservice.controller;

import com.example.webservice.dto.*;
import com.example.webservice.entity.User;
import com.example.webservice.service.GeminiService;
import com.example.webservice.service.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Map;

@RestController
@RequiredArgsConstructor
@RequestMapping("/user")
@CrossOrigin
public class UserController {
    private final UserService userService;
    private final GeminiService geminiService;

    @PostMapping("/changePassword")
    public ResponseEntity<String> changePassword(@RequestBody UserChangePassword userChangePassword){
        return ResponseEntity.ok().body(userService.changPassword(userChangePassword));
    }

    @PostMapping("/createRoom")
    public ResponseEntity<String> createRoom(@RequestBody CreateRoomDto createRoomDto) throws Exception {
        return ResponseEntity.ok().body(userService.createRoom(createRoomDto));
    }

    @PostMapping("/joinRoom")
    public ResponseEntity<UserDto> joinRoom(@RequestBody JoinRoomDto joinRoomDto) throws Exception {
        return ResponseEntity.ok().body(userService.joinRoom(joinRoomDto));
    }

    @PostMapping("/outRoom")
    public ResponseEntity<String> outRoom(@RequestBody JoinRoomDto joinRoomDto) throws Exception {
        return ResponseEntity.ok().body(userService.outRoom(joinRoomDto));
    }

    @GetMapping("/allRoom")
    public ResponseEntity<List<CreateRoomDto>> getAllRoom(){
        return ResponseEntity.ok().body(userService.getAllRoom());
    }

    @PostMapping("/{username}/win")
    public ResponseEntity<String> win(@PathVariable String username, @RequestBody RoomDto roomDto) throws Exception {
        return ResponseEntity.ok(userService.win(username, roomDto.getType()));
    }

    @PostMapping("/{username}/lose")
    public ResponseEntity<String> lose(@PathVariable String username, @RequestBody RoomDto roomDto) throws Exception {
        return ResponseEntity.ok(userService.lose(username, roomDto.getType()));
    }

    @GetMapping("/money")
    public ResponseEntity<Double> getMoney(@RequestParam("username") String username){
        return ResponseEntity.ok(userService.getMoney(username));
    }

    @GetMapping("")
    public ResponseEntity<UserDto> getUser(@RequestParam("username") String username){
        return ResponseEntity.ok(userService.getUser(username));
    }

    @GetMapping("/room/info")
    public ResponseEntity<UserInfoRoomDto> getInfo(@RequestParam("roomId") String roomId){
        return ResponseEntity.ok(userService.getInfoUserInRoom(roomId));
    }

    @GetMapping("/addMoney")
    public ResponseEntity<String> addMoney(@RequestParam("username") String username){
        return ResponseEntity.ok(userService.addMoney(username));
    }

    @GetMapping("/subMoney")
    public ResponseEntity<String> subMoney(@RequestParam("username") String username){
        return ResponseEntity.ok(userService.subMoney(username));
    }

    @GetMapping("/info")
    public ResponseEntity<UserDto> getInfoUser(@RequestParam("username") String username){
        return ResponseEntity.ok(userService.getInfoUser(username));
    }

    @PutMapping("/edit")
    public ResponseEntity<String> editUser(@RequestBody UserDto userDto, @RequestParam("username") String username){
        return ResponseEntity.ok(userService.editInfo(userDto, username));
    }

    @PostMapping("/gemini")
    public String generateContent(@RequestBody Map<String, String> request) {
        String text = request.get("text");
        return geminiService.generateContent(text);
    }
}
