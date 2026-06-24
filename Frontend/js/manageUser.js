// --- manageUser.js ---
async function loadUsers() {
  if (!requireAdmin()) return;

  const token = getToken();

  try {
    const response = await fetch(`${API_BASE_URL}/Account/view_all_user`, {
      method: "GET",
      headers: { Authorization: `Bearer ${token}` },
    });

    if (response.ok) {
      const data = await response.json();
      displayUsers(data);
    } else if (response.status === 404) {
      document.getElementById("userListContainer").innerHTML =
        "<h3>No users in the database yet</h3>";
    } else {
      console.error("Something went wrong with the C# server.");
    }
  } catch (error) {
    console.error("The API is turned off or blocked by CORS.", error);
  }
}

function displayUsers(userArray) {
  const container = document.getElementById("userListContainer");
  container.innerHTML = "";

  userArray.forEach(function (appUser) {
    const userHtml = `
      <div class="user-row">
          <div class="user-info">
              <p class="user-name">${appUser.firstName} ${appUser.lastName}</p>
              <p class="user-email">Username: ${appUser.username} | Email: ${appUser.email}</p>
          </div>
          <div class="user-actions">
             <button class="edit-btn" onclick="window.location.href='editUser.html?id=${appUser.appUserId}'">Edit</button>
             <button class="delete-btn" onclick="deleteUser('${appUser.appUserId}')">Delete</button>
          </div>
      </div>
    `;
    container.innerHTML += userHtml;
  });
}

async function deleteUser(userId) {
  const isConfirmed = confirm(
    "Are you sure you want to delete this user? This cannot be undone.",
  );
  if (!isConfirmed) return;

  const token = getToken();

  try {
    const response = await fetch(
      `${API_BASE_URL}/Account/Delete_User/${userId}`,
      {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      },
    );

    if (response.ok) {
      alert("Account Successfully Deleted");
      loadUsers();
    } else {
      const errorText = await response.text();
      alert("Failed to delete user: " + errorText);
    }
  } catch (error) {
    console.error("The API is turned off or blocked by CORS.", error);
  }
}

loadUsers();
