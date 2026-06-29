// --- verify-email.js ---
const form = document.getElementById("verifyForm");

form.addEventListener("submit", async function (event) {
  event.preventDefault();

  // 1. Grab the email we saved during registration
  const savedEmail = localStorage.getItem("pendingVerificationEmail");
  const otpCode = document.getElementById("otpCode").value;

  if (!savedEmail) {
    alert("Session expired. Please try registering or logging in again.");
    window.location.href = "login.html";
    return;
  }

  const verifyBtn = document.getElementById("verifyBtn");
  verifyBtn.innerHTML = "Verifying...";
  verifyBtn.disabled = true;

  const payload = {
    email: savedEmail,
    otpCode: otpCode,
  };

  try {
    const response = await fetch(`${API_BASE_URL}/account/verify-email`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    });

    if (response.ok) {
      const data = await response.json();

      // SUCCESS! We finally got the Master Key (Token). Save it!
      localStorage.setItem("ecommerceToken", data.token);

      // Clear the temporary email
      localStorage.removeItem("pendingVerificationEmail");

      alert("Email verified successfully! Welcome to the store.");

      // Route them to the correct dashboard based on role
      if (data.role === "Admin") {
        window.location.href = "adminDashboard.html";
      } else {
        window.location.href = "home.html";
      }
    } else {
      // E.g., "Invalid OTP Code" or "OTP has expired"
      const errorText = await response.text();
      alert(errorText);
    }
  } catch (error) {
    console.error("Server error:", error);
    alert("Could not connect to the server.");
  } finally {
    verifyBtn.innerHTML = "Verify & Login";
    verifyBtn.disabled = false;
  }
});

// --- NEW RESEND OTP LOGIC ---
async function resendOTP() {
  const savedEmail = localStorage.getItem("pendingVerificationEmail");

  if (!savedEmail) {
    alert("Session expired. Please try registering again.");
    window.location.href = "register.html";
    return;
  }

  const resendText = document.getElementById("resendText");

  // Visual feedback to prevent spam clicking
  resendText.innerText = "Sending...";
  resendText.style.pointerEvents = "none";
  resendText.style.color = "gray";

  try {
    const response = await fetch(`${API_BASE_URL}/account/resend-otp`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      // Note: We only need to send the email to trigger the resend
      body: JSON.stringify({ email: savedEmail }),
    });

    if (response.ok) {
      alert("A new verification code has been sent to your email!");
    } else {
      const errorText = await response.text();
      alert(errorText);
    }
  } catch (error) {
    console.error("Server error:", error);
    alert("Could not connect to the server to resend OTP.");
  } finally {
    // Reset the text back to normal so they can click it again if needed
    resendText.innerText = "Resend OTP";
    resendText.style.pointerEvents = "auto";
    resendText.style.color = "#007bff";
  }
}
