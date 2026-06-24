// --- global.js ---

// 1. THE MASTER URL
const API_BASE_URL = "https://localhost:7047/api";

// 2. THE MASTER TOKEN GRABBER
function getToken() {
  // This is the absolute source of truth for your token!
  return localStorage.getItem("ecommerceToken");
}

// 3. ADMIN GUARD
function requireAdmin() {
  const token = getToken();

  // 1. Check if they are even logged in
  if (!token) {
    alert("Access Denied: You must be logged in to view this page.");
    window.location.href = "../pages/login.html";
    return false;
  }

  try {
    // 2. Crack open the JWT Token
    const payload = JSON.parse(atob(token.split(".")[1]));

    // 3. Find the Role
    const userRole =
      payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
      payload.role;

    // 4. Check if they are an Admin
    if (userRole !== "Admin") {
      alert("Access Denied: You do not have Admin privileges.");
      window.location.href = "../pages/home.html";
      return false;
    }

    return true;
  } catch (error) {
    alert("Session corrupted. Please log in again.");
    logout();
    return false;
  }
}

// 4. THE MASTER LOGOUT
function logout() {
  localStorage.removeItem("ecommerceToken");
  window.location.href = "../pages/login.html";
}

// 5. THE MASTER DROPDOWN LOGIC
document.addEventListener("DOMContentLoaded", function () {
  const accountBtn = document.getElementById("accountBtn");
  const accountDropdown = document.getElementById("accountDropdown");

  if (accountBtn && accountDropdown) {
    accountBtn.addEventListener("click", function (event) {
      event.stopPropagation();
      accountDropdown.classList.toggle("show");
    });

    window.addEventListener("click", function () {
      if (accountDropdown.classList.contains("show")) {
        accountDropdown.classList.remove("show");
      }
    });
  }
});

// 6. THE AUTH GUARD (The Bouncer)
// 6. THE AUTH GUARD (The Bouncer)
function enforceAuthGuard() {
  const token = getToken();
  const currentPage = window.location.pathname.toLowerCase();

  // NEW: Handle the raw domain landing (e.g., www.yourstore.com/ or localhost:5500/)
  if (currentPage === "/" || currentPage === "/index.html") {
    // Automatically redirect them to the storefront so they can browse
    window.location.href = "pages/home.html";
    return; // Stop the rest of the function
  }

  const publicPages = [
    "verify-email.html",
    "login.html",
    "register.html",
    "home.html",
    "products.html",
  ];

  const isPublicPage = publicPages.some((page) => currentPage.includes(page));

  if (!token && !isPublicPage) {
    alert("You must be logged in to view this page.");
    window.location.href = "../pages/login.html";
  }
}

// 7. DYNAMIC NAV SWITCHER (Login vs Account Menu)
function updateDynamicNav() {
  const token = getToken(); // FIXED: Now uses the correct token!

  const loginBtn = document.getElementById("navLoginBtn");
  const accountMenu = document.getElementById("navAccountMenu");

  if (loginBtn && accountMenu) {
    if (token) {
      // User IS logged in: Show Account Menu inline!
      loginBtn.style.display = "none";
      accountMenu.style.display = "inline-block"; // <--- CHANGED THIS FROM "block"
    } else {
      // User is NOT logged in: Show Login Button inline!
      loginBtn.style.display = "inline-block"; // <--- CHANGED THIS FROM "inline"
      accountMenu.style.display = "none";
    }
  }
}

// --- RUN ON LOAD ---
// Run the bouncer immediately
enforceAuthGuard();

// Run the nav switcher after the HTML has finished loading
document.addEventListener("DOMContentLoaded", updateDynamicNav);
