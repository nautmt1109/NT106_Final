# Sử dụng image chính thức của Maven để build ứng dụng
FROM maven:3.8.3-openjdk-17-slim AS build

# Đặt thư mục làm việc
WORKDIR /app

# Copy toàn bộ mã nguồn vào container
COPY . .

# Thực hiện lệnh mvn clean install để build ứng dụng
RUN mvn clean package -DskipTests install

# Sử dụng image chính thức của OpenJDK 17 để chạy ứng dụng
FROM openjdk:17-jdk-slim

# Đặt thư mục làm việc
WORKDIR /app

# Copy file jar từ giai đoạn build sang giai đoạn run
COPY --from=build /app/target/*.jar app.jar

# Khai báo lệnh chạy ứng dụng
CMD ["java", "-jar", "app.jar"]

# Mở cổng 8080
EXPOSE 8080