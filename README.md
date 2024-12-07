# Thành viên nhóm:

1. Nguyễn Đình Mạnh - B21DCCN511
2. Bùi Huy Hoàng - B21DCCN055

# Các Usecase chính

1. Join phòng: Cho phép Player nhập mã phòng và chờ host bắt đầu game
2. Chơi game: Cho phép Player đu dây qua các chốt và va chạm với các Platform
3. Hiển thị Leaderboard: Xem bảng xếp hạng khi đang trên đường đua và sau khi về đích

# Công nghệ sử dụng

1. Unity Engiene
2. Photon FrameWork:

- Hỗ trợ cả mô hình P2P và Dedicated Server, đồng bộ hóa trạng thái game giữa các người chơi thông qua Remote Procedure Calls (RPCs),
- Cung cấp API dễ dàng để quản lý các phòng và người chơi trong phòng
- Tích hợp với các dịch vụ backend như PlayFab để lưu trữ và hiển thị bảng xếp hạng (leaderboard)

# Demo (Gameplay single player)

![Minh họa Gameplay](Figures/demo.png)

# Triển khai

1. Tính năng điều khiển nhân vật

- Người chơi có thể điều khiển nhân vật bằng cách nhấn Space
- Hệ thống hỗ trợ người chơi sử dụng cử chỉ để điều khiển nhân vật:
  - Xây dựng hệ thống nhận diện cử chỉ của người chơi sử dụng Python, thư viện OpenCV và CvZone.
  - Ứng dụng nhận diện cử chỉ giao tiếp với gameplay qua giao thức UDP. Do tính năng này chỉ nằm trên máy cục bộ của người chơi nên sẽ giap tiếp qua localhost tại cổng 6060.
  - Sau khi build thành file .exe, Unity Engiene tạo 1 tiến trình chạy nền cho chương trình nhận diện cử chỉ và đảm bảo khi thoát ứng dụng thì cũng sẽ hủy được hết tiến trình chạy nền.

2. Tính năng Xác thực người dùng

   Khi một người chơi mới hoặc người chơi quay lại đăng nhập vào game, Unity Authentication sẽ tạo các token và ID sau:

   - `PlayerID` (một chuỗi ngẫu nhiên 28 ký tự): dùng để nhận diện người chơi quay lại và người chơi mới trên các thiết bị khác nhau và các nhà cung cấp bên ngoài.
   - Session token: dùng để xác thực lại người dùng sau khi phiên làm việc hết hạn.
   - Authentication token: chứa `PlayerID` và được sử dụng để ngăn chặn tài khoản khác ghi đè danh tính.

   Trong project này sẽ sử dụng xác thực ẩn danh để tiện cho việc testing. Với cách này thì khi đăng nhập vào game sẽ không có authentication token, nên không thể login vào tài khoản và xem player data sau khi xoá game.

![Minh họa Gameplay](Figures/new_authen.png)

3. Tính năng sảnh chờ - Lobby

- Lobby cung cấp một nơi để người chơi có thể tìm thấy và kết nối với nhau. Lobby có thể duy trì trong suốt 1 game session cho phép một người chơi có thể join lại sau 1 game session sau khi thoát hoặc sau khi bị mất kết nối đột ngột
- Lobby sẽ được sử dụng để tạo phòng cho người chơi join vào
- Lobby code hay join code sẽ được cung cấp thông qua relay server ở phần dưới

  ![Minh họa Gameplay](Figures/new_lobby.png)

4. Relay
   Relay là giải pháp mạng P2P giúp developer dễ dàng thiết lập kết nối giữa người chơi đồng thời bảo vệ quyền riêng tư. Thay vì sử dụng server chuyên dụng, Relay cung cấp kết nối thông qua một máy chủ Relay chung đóng vai trò làm proxy.

   ![Minh họa Gameplay](Figures/relay.png)

   4.1. Netcode for GameObjects

   - Netcode for GameObjects là thư viện mạng cấp cao được xây dựng cho Unity để trừu tượng hóa logic mạng. Trong project sẽ sử dụng cùng Relay để việc gửi nhận dữ liệu qua được đóng gói dễ dàng hơn.

   - Khi thiết lập kết nối tới Relay server, host cần thiết lập allocation. Đây là một yêu cầu từ host gửi đến Allocations service để dành slot trên Relay server cho host và số lượng player tối đa mà host mong muốn. Khi một allocation đã tồn tại thì cần đảm bảo rằng mọi traffic gửi nhận dữ liệu đến từ Netcode for GameObjects đều đi qua Relay, sau đó có thể gửi nhận các dữ liệu của GameObjects. Ví dụ:

   ```cs
   // Retrieve the Unity transport used by the NetworkManager
    UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    transport.SetRelayServerData(new RelayServerData(allocation, connectionType:"dtls"));
   ```

# Mô tả hệ thống 
1. Chức năng tạo phòng: Hỗ trợ nhiều người chơi, tạo và tham gia phòng với nhiều người khác khi có mã phòng

2. Chức năng chọn nhân vật:
- Hệ thống hỗ trợ chọn nhân vật trong lobby sử dụng UI / nhận diện cử chỉ với 2 ngón cái và ngón út tùy vào hướng mỗi ngón
- Dữ liệu chọn nhân vật giữa các người chơi trong lobby được đồng bộ qua LobbyService
- Chỉ khi nào tất cả người chơi trong phòng sẵn sàng mới có thể vào gameplay

3. Gameplay
- Người chơi có thể điều khiển nhân vật qua nút Space
- Hệ thống hỗ trợ người chơi sử dụng cử chỉ nắm / mở bàn tay để đu dây
