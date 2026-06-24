const urlParams = new URLSearchParams(window.location.search);
const userId = urlParams.get("id");

async function loadUser() {
  const token = getToken();

  try {
    const response = await fetch(`${API_BASE_URL}/Account/myProfile`, {
      method: "GET",
      headers: { Authorization: `Bearer ${token}` },
    });

    if (response.ok) {
      const data = await response.json();

      document.getElementById("editUsername").value = data.username;
      document.getElementById("editEmail").value = data.email;
    } else {
      console.error("Failed to load user. Status:", response.status);
    }
  } catch (error) {
    console.error("Server is offline.", error);
  }
}

const form = document.getElementById("editProfileForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  const token = getToken();

  const updateProfile = {
    username: document.getElementById("editUsername").value,
    // email: document.getElementById("editEmail").value,
  };

  try {
    const response = await fetch(`${API_BASE_URL}/Account/userProfileUpdate`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(updateProfile),
    });

    if (response.ok) {
      alert("User Updated");
      window.location.href = "home.html";
    } else {
      console.error("Failed to update user. Status:", response.status);
    }
  } catch (error) {
    console.error("Server is offline.", error);
  }
});

loadUser();
