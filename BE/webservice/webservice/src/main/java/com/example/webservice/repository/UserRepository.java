package com.example.webservice.repository;

import com.example.webservice.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface UserRepository extends JpaRepository<User, Long> {
    @Query(value = "select * from tb_user where email = ?1", nativeQuery = true)
    Optional<User> findByEmail(String email);

    Optional<User> findByVerifyToken(String token);

    Optional<User> findByUsername(String username);
}
