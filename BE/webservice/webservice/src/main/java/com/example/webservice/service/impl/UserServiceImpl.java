package com.example.webservice.service.impl;

import com.example.webservice.dto.*;

import com.example.webservice.entity.Room;
import com.example.webservice.entity.User;
import com.example.webservice.repository.RoomRepository;
import com.example.webservice.repository.UserRepository;
import com.example.webservice.service.UserService;
import jakarta.transaction.Transactional;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {
    private final UserRepository userRepository;
    private final RoomRepository roomRepository;

    @Override
    public String changPassword(UserChangePassword userChangePassword) {
        Optional<User> user = userRepository.findByUsername(userChangePassword.getUsername());
        if (user.isPresent()){
            User _user = user.get();
            _user.setPassword(userChangePassword.getPassword());
            userRepository.save(_user);
            return "Password change successfully";
        }
        return "Error";
    }

    @Override
    public String createRoom(CreateRoomDto createRoomDto) throws Exception {
        try {
            User user = userRepository.findByUsername(createRoomDto.getUsername()).orElseThrow(() -> new Exception("Cannot find username"));
            Room room = new Room();
            room.setRoomId(createRoomDto.getRoomId());
            room.setEnable(true);
            room.setTypeMoney(createRoomDto.getTypeMoney());
            List<User> userList = new ArrayList<>();
            userList.add(user);
            room.setUserList(userList);
            roomRepository.save(room);
            return "New room is created";
        }
        catch (Exception e){
            return "Error";
        }
    }

    @Override
    public UserDto joinRoom(JoinRoomDto joinRoomDto) throws Exception {
        User user = userRepository.findByUsername(joinRoomDto.getUsername()).orElseThrow(() -> new Exception("Cannot find username"));
        Room room = roomRepository.findByRoomId(joinRoomDto.getRoomId());
        room.getUserList().add(user);
        roomRepository.save(room);
        UserDto userDto = new UserDto();
        userDto.setMoney(user.getMoney());
        userDto.setUsername(user.getUsername());
        return userDto;
    }

    @Override
    public List<CreateRoomDto> getAllRoom() {
        List<Room> roomList = roomRepository.findAll();
        List<CreateRoomDto> createRoomDtoList = new ArrayList<>();
        for (Room room : roomList){
            CreateRoomDto roomDto = new CreateRoomDto();
            roomDto.setRoomId(room.getRoomId());
            roomDto.setTypeMoney(room.getTypeMoney());
            roomDto.setNumberPeople(room.getUserList().size());
            createRoomDtoList.add(roomDto);
        }
        return createRoomDtoList;
    }

    @Override
    public String lose(String username, int type) throws Exception {
        User user = userRepository.findByUsername(username).orElseThrow(() -> new Exception("Cannot find username"));
        double money = user.getMoney() - type;
        if (money < 0){
            return "You don't have enough money";
        }
        user.setMoney(money);
        userRepository.save(user);
        return "${username} win";
    }

    @Override
    public double getMoney(String username) {
        User user = userRepository.findByUsername(username).orElse(null);
        return user.getMoney();
    }

    @Override
    public String outRoom(JoinRoomDto joinRoomDto) {
        try {
            String username = joinRoomDto.getUsername();
            User user = userRepository.findByUsername(username).orElseThrow(() -> new Exception("Cannot find username"));
            String roomID = joinRoomDto.getRoomId();
            Room room = roomRepository.findByRoomId(roomID);
            List<User> userList = room.getUserList();
            userList.remove(user);
            room.setUserList(userList);
            roomRepository.save(room);
            return "success";
        }
        catch (Exception e){
            return "Error";
        }
    }

    @Override
    public UserDto getUser(String username) {
        User user = userRepository.findByUsername(username).orElse(null);
        UserDto userDto = new UserDto();
        assert user != null;
        userDto.setUsername(user.getUsername());
        userDto.setMoney(user.getMoney());
        userDto.setEmail(user.getEmail());
        userDto.setMatchWin(user.getMatchWin());
        userDto.setMatchLose(user.getMatchLose());
        return userDto;
    }

    @Override
    public UserInfoRoomDto getInfoUserInRoom(String roomId) {
        Room room = roomRepository.findByRoomId(roomId);
        List<User> userList = room.getUserList();
        UserInfoRoomDto userInfoRoomDto = new UserInfoRoomDto();
        if (userList.size() == 1){
            User user = userList.get(0);
            userInfoRoomDto.setUsername1(user.getUsername());
            userInfoRoomDto.setMoney1(user.getMoney());
        }
        else if (userList.size() == 2){
            User user1 = userList.get(0);
            User user2 = userList.get(1);
            userInfoRoomDto.setUsername1(user1.getUsername());
            userInfoRoomDto.setMoney1(user1.getMoney());
            userInfoRoomDto.setUsername2(user2.getUsername());
            userInfoRoomDto.setMoney2(user2.getMoney());
        }
        return userInfoRoomDto;
    }

    @Override
    @Transactional
    public String addMoney(String username) {
        User user = userRepository.findByUsername(username).orElse(null);
        assert user != null;
        double money = user.getMoney() + 500;
        int matchWin = user.getMatchWin() + 1;
        user.setMoney(money);
        user.setMatchWin(matchWin);
        userRepository.save(user);
        return "" +money;
    }

    @Override
    @Transactional
    public String subMoney(String username) {
        User user = userRepository.findByUsername(username).orElse(null);
        assert user != null;
        double money = user.getMoney() - 500;
        int matchLose = user.getMatchLose() + 1;
        user.setMoney(money);
        user.setMatchLose(matchLose);
        userRepository.save(user);
        return "" +money;
    }

    @Override
    public UserDto getInfoUser(String username) {
        User user = userRepository.findByUsername(username).orElse(null);
        UserDto userDto = new UserDto();
        assert user != null;
        userDto.setUsername(user.getUsername());
        userDto.setMoney(user.getMoney());
        userDto.setEmail(user.getEmail());
        userDto.setMatchWin(user.getMatchWin());
        userDto.setMatchLose(user.getMatchLose());
        userDto.setPassword(user.getPassword());
        return userDto;
    }

    @Override
    public String editInfo(UserDto userDto, String username) {
        User user = userRepository.findByUsername(username).orElse(null);
        assert user != null;
        user.setEmail(userDto.getEmail());
        user.setUsername(userDto.getUsername());
        user.setPassword(userDto.getPassword());
        userRepository.save(user);
        return "success";
    }

    @Override
    public String win(String username, int type) throws Exception {
        User user = userRepository.findByUsername(username).orElseThrow(() -> new Exception("Cannot find username"));
        double money = user.getMoney() + type;
        user.setMoney(money);
        userRepository.save(user);
        return "${username} lose";
    }
}
