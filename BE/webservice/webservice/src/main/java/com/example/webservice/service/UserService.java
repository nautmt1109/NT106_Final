package com.example.webservice.service;

import com.example.webservice.dto.*;
import com.example.webservice.entity.User;

import java.util.List;

public interface UserService {
    String changPassword(UserChangePassword userChangePassword);

    String createRoom(CreateRoomDto createRoomDto) throws Exception;

    UserDto joinRoom(JoinRoomDto joinRoomDto) throws Exception;

    List<CreateRoomDto> getAllRoom();

    String win(String username, int type) throws Exception;

    String lose(String username, int type) throws Exception;

    double getMoney(String username);

    String outRoom(JoinRoomDto joinRoomDto);

    UserDto getUser(String username);

    UserInfoRoomDto getInfoUserInRoom(String roomId);

    String addMoney(String username);

    String subMoney(String username);

    UserDto getInfoUser(String username);

    String editInfo(UserDto userDto, String username);
}
