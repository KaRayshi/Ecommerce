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

      const userRole = data.role || "";
      if (userRole.toLowerCase() === "user") {
        window.location.href = "../pages/home.html";
      } else {
        window.location.href = "../pages/adminDashboard.html";
      }
    } else {
      // === NEW LOGIN ERROR HANDLING ===
      const errorText = await response.text();

      // Check if C# specifically rejected them because of an unverified email
      if (response.status === 401 && errorText.includes("verified")) {
        // Save the email they just tried to log in with
        localStorage.setItem("pendingVerificationEmail", user.usernameOrEmail);
        alert(
          "Your email has not been verified yet. Redirecting to the verification page...",
        );
        window.location.href = "verify-email.html";
      } else {
        alert(errorText || "Invalid username or password.");
      }
    }
  } catch (error) {
    console.error("The API is probably turned off.", error);
    alert("The API is probably turned off.", error);
  } finally {
    loginButton.innerHTML = "Login";
    loginButton.disabled = false;
  }
});
