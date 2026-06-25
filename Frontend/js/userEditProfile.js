const urlParams = new URLSearchParams(window.location.search);
const userId = urlParams.get("id");

// 1. We will store the user's email here so the password reset knows where to send the OTP
let currentUserEmail = "";

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

      // Save the email to our global variable!
      currentUserEmail = data.email;
    } else {
      console.error("Failed to load user. Status:", response.status);
    }
  } catch (error) {
    console.error("Server is offline.", error);
  }
}

// --- UPDATE USERNAME LOGIC ---
const form = document.getElementById("editProfileForm");
form.addEventListener("submit", async function (event) {
  event.preventDefault();
  const token = getToken();
  const updateProfile = {
    username: document.getElementById("editUsername").value,
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
      alert("Profile Updated Successfully!");
      // window.location.reload(); // Reload to show new username
    } else {
      console.error("Failed to update user. Status:", response.status);
    }
  } catch (error) {
    console.error("Server is offline.", error);
  }
});

// --- CHANGE PASSWORD LOGIC (STAGE 1: REQUEST OTP) ---
const requestOtpBtn = document.getElementById("requestOtpBtn");
const passwordResetSection = document.getElementById("passwordResetSection");

requestOtpBtn.addEventListener("click", async function () {
  requestOtpBtn.innerHTML = "Sending OTP...";
  requestOtpBtn.disabled = true;

  try {
    const response = await fetch(
      `${API_BASE_URL}/account/request-password-reset`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: currentUserEmail }),
      },
    );

    if (response.ok) {
      alert("Security code sent! Please check your email.");
      requestOtpBtn.style.display = "none";
      passwordResetSection.style.display = "block";
    } else {
      const errorText = await response.text();
      alert(errorText || "Failed to send OTP.");
      requestOtpBtn.innerHTML = "Request OTP to Change Password";
      requestOtpBtn.disabled = false;
    }
  } catch (error) {
    console.error(error);
    alert("Server offline.");
    requestOtpBtn.innerHTML = "Request OTP to Change Password";
    requestOtpBtn.disabled = false;
  }
});

// --- CHANGE PASSWORD LOGIC (STAGE 2: CONFIRM NEW PASSWORD) ---
const confirmPasswordChangeBtn = document.getElementById(
  "confirmPasswordChangeBtn",
);

confirmPasswordChangeBtn.addEventListener("click", async function () {
  const otp = document.getElementById("profileOtpCode").value;
  const newPass = document.getElementById("profileNewPassword").value;

  confirmPasswordChangeBtn.innerHTML = "Updating Password...";
  confirmPasswordChangeBtn.disabled = true;

  try {
    const response = await fetch(
      `${API_BASE_URL}/account/verify-otp-and-reset`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: currentUserEmail,
          otpCode: otp,
          newPassword: newPass,
        }),
      },
    );

    if (response.ok) {
      alert(
        "Password successfully updated! You can now use it on your next login.",
      );

      // Clean up the UI
      passwordResetSection.style.display = "none";
      requestOtpBtn.style.display = "block";
      requestOtpBtn.innerHTML = "Request OTP to Change Password";
      document.getElementById("profileOtpCode").value = "";
      document.getElementById("profileNewPassword").value = "";
      requestOtpBtn.disabled = false;
    } else {
      const errorText = await response.text();
      alert(errorText || "Failed to update password. Check your OTP.");
    }
  } catch (error) {
    console.error(error);
    alert("Server offline.");
  } finally {
    confirmPasswordChangeBtn.innerHTML = "Confirm Password Change";
    confirmPasswordChangeBtn.disabled = false;
  }
});

// Run on load
loadUser();
