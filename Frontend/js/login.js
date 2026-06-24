// --- login.js ---
const form = document.getElementById("loginForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  const usernameOrEmailValue = document.getElementById("usernameOrEmail").value;
  const passwordValue = document.getElementById("password").value;
  const loginButton = document.querySelector(".submit-btn");

  loginButton.innerHTML = "Please wait...";
  loginButton.disabled = true;

  const user = {
    usernameOrEmail: usernameOrEmailValue,
    password: passwordValue,
  };

  try {
    const response = await fetch(`${API_BASE_URL}/account/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(user),
    });

    if (response.ok) {
      const data = await response.json();
      localStorage.setItem("ecommerceToken", data.token);

      console.log("Server returned role:", data.role);

      const userRole = data.role || "";
      console.log("The role is:", userRole);

      if (userRole.toLowerCase() === "user") {
        window.location.href = "../pages/home.html";
      } else {
        window.location.href = "../pages/adminDashboard.html";
      }
    } else {
      alert("Invalid username or password.");
    }
  } catch (error) {
    console.error("The API is probably turned off.", error);
    alert("The API is probably turned off.", error);
  } finally {
    loginButton.innerHTML = "Login";
    loginButton.disabled = false;
  }
});
