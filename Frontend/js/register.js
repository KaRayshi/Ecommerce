// --- register.js ---
const form = document.getElementById("registerForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  const firstNameValue = document.getElementById("firstName").value;
  const lastNameValue = document.getElementById("lastName").value;
  const usernameValue = document.getElementById("username").value;
  const emailValue = document.getElementById("email").value;
  const passwordValue = document.getElementById("password").value;

  const registerButton = document.querySelector(".submit-btn");
  registerButton.innerHTML = "Please wait...";
  registerButton.disabled = true;

  const newUser = {
    firstName: firstNameValue,
    lastName: lastNameValue,
    username: usernameValue,
    email: emailValue,
    password: passwordValue,
  };

  try {
    const response = await fetch(`${API_BASE_URL}/api/account/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(newUser),
    });

    if (response.ok) {
      localStorage.setItem("pendingVerificationEmail", emailValue);

      alert(
        "Registration successful! Please check your email for the 6-digit code.",
      );
      window.location.href = "verify-email.html";
    } else {
      const errorData = await response.json();
      let errorMessage = "Registration failed:\n";

      if (Array.isArray(errorData)) {
        errorData.forEach((err) => (errorMessage += `- ${err.description}\n`));
      } else {
        errorMessage += JSON.stringify(errorData);
      }
      alert(errorMessage);
    }
  } catch (error) {
    console.error("The API is probably turned off.", error);
  } finally {
    registerButton.innerHTML = "Register";
    registerButton.disabled = false;
  }
});
