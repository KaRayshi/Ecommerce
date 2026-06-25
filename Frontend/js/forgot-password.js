// --- forgot-password.js ---
const requestForm = document.getElementById("requestOtpForm");
const resetForm = document.getElementById("resetPasswordForm");

let userEmail = ""; // Keeps track of the email across both forms

// --- STAGE 1: Requesting the OTP ---
requestForm.addEventListener("submit", async function (event) {
  event.preventDefault();

  userEmail = document.getElementById("resetEmail").value;
  const requestBtn = document.getElementById("requestBtn");

  requestBtn.innerHTML = "Sending...";
  requestBtn.disabled = true;

  try {
    // Change endpoint URL if yours is named differently (e.g., /account/forgot-password)
    const response = await fetch(
      `${API_BASE_URL}/account/request-password-reset`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: userEmail }),
      },
    );

    if (response.ok) {
      alert("An OTP code has been sent to your email!");
      // Hide the request form and show the reset form
      requestForm.style.display = "none";
      resetForm.style.display = "block";
    } else {
      const errorText = await response.text();
      alert(
        errorText ||
          "Failed to initiate password reset. Ensure the email is correct.",
      );
    }
  } catch (error) {
    console.error("Connection error:", error);
    alert("Could not reach the server.");
  } finally {
    requestBtn.innerHTML = "Send Code";
    requestBtn.disabled = false;
  }
});

// --- STAGE 2: Resetting the Password ---
resetForm.addEventListener("submit", async function (event) {
  event.preventDefault();

  const otpCode = document.getElementById("resetOtp").value;
  const newPassword = document.getElementById("newPassword").value;
  const resetBtn = document.getElementById("resetBtn");

  resetBtn.innerHTML = "Updating...";
  resetBtn.disabled = true;

  // This payload matches your backend DTO structure for resetting passwords
  const payload = {
    email: userEmail,
    otpCode: otpCode,
    newPassword: newPassword,
  };

  try {
    const response = await fetch(
      `${API_BASE_URL}/account/verify-otp-and-reset`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      },
    );

    if (response.ok) {
      alert("Password updated successfully! Redirecting to login page...");
      window.location.href = "login.html";
    } else {
      const errorText = await response.text();
      alert(
        errorText || "Failed to reset password. Please verify your OTP code.",
      );
    }
  } catch (error) {
    console.error("Connection error:", error);
    alert("Could not reach the server.");
  } finally {
    resetBtn.innerHTML = "Update Password";
    resetBtn.disabled = false;
  }
});
