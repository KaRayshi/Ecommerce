// --- editUser.js ---
const urlParams = new URLSearchParams(window.location.search);
const UserId = urlParams.get("id");

async function loadUser() {
  if (!requireAdmin()) return;

  const token = getToken();
  try {
    const response = await fetch(
      `${API_BASE_URL}/Account/view_user/${UserId}`,
      {
        method: "GET",
        headers: { Authorization: `Bearer ${token}` },
      },
    );

    if (response.ok) {
      const data = await response.json();
      document.getElementById("editUsername").value = data.username;
      document.getElementById("editFirstName").value = data.firstName;
      document.getElementById("editLastName").value = data.lastName;
    } else {
      console.error("Failed to load user. Status:", response.status);
    }
  } catch (error) {
    console.error("Server is offline.", error);
  }
}

const form = document.getElementById("editUserForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  const saveBtn = document.getElementById("saveUserBtn");
  saveBtn.innerHTML = "Updating...";
  saveBtn.disabled = true;

  const updatedUser = {
    username: document.getElementById("editUsername").value,
    firstName: document.getElementById("editFirstName").value,
    lastName: document.getElementById("editLastName").value,
  };

  const token = getToken();

  try {
    const response = await fetch(
      `${API_BASE_URL}/Account/update_user/${UserId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(updatedUser),
      },
    );

    if (response.ok) {
      alert("User Updated");
      window.location.href = "manageUser.html";
    } else {
      console.error("Failed to update user. Status:", response.status);
    }
  } catch (error) {
    console.log("Server is offline", error);
  } finally {
    saveBtn.innerHTML = "Update User";
    saveBtn.disabled = false;
  }
});

loadUser();
