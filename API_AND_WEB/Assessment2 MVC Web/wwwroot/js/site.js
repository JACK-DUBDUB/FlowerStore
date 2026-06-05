// ======================================================
// Store shared client helpers (cart, toast, utils)
// Branding comes from SiteSettings via Razor
// ======================================================

const PETAL_API_BASE = 'https://localhost:7209';
const PETAL_VERSION_HEADER = 'FlowerStore-API-Version';
const PETAL_API_VERSION = '1.0';

// Show a toast notification (success / error)
function showToast(message, type = 'success', timeoutMs = 3200) {
    const container = document.getElementById('toast-container');
    if (!container) {
        console.log(message);
        return;
    }
    const toast = document.createElement('div');
    toast.className = `toast ${type === 'error' ? 'error' : 'success'}`;
    toast.style.minWidth = '260px';
    toast.innerHTML = `
        <div class="toast-body d-flex">
            <div class="me-2">${type === 'success' ? '✅' : '⚠️'}</div>
            <div class="flex-1">${message}</div>
            <button type="button" class="btn-close btn-close-white ms-2" style="font-size:.7rem" aria-label="Close"></button>
        </div>`;
    container.appendChild(toast);

    // close button
    const close = toast.querySelector('.btn-close');
    if (close) close.addEventListener('click', () => toast.remove());

    if (timeoutMs > 0) {
        setTimeout(() => {
            if (toast.parentNode) toast.parentNode.removeChild(toast);
        }, timeoutMs);
    }
    return toast;
}

// Escape for safe innerHTML
function escapeHtml(str) {
    if (!str) return '';
    return String(str).replace(/[&<>"']/g, m => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[m]));
}

// Add item to cart (localStorage)
function addToCart(product) {
    if (!product || !product.id) return;

    let cart = [];
    try { cart = JSON.parse(localStorage.getItem('petalCart') || '[]'); } catch (e) {}

    const existing = cart.findIndex(i => i.id === product.id);
    if (existing >= 0) {
        cart[existing].qty = (cart[existing].qty || 1) + 1;
    } else {
        cart.push({
            id: product.id,
            name: product.name,
            price: product.price,
            location: product.storeLocation || product.StoreLocation || '',
            emoji: product.emoji || '🌸',
            qty: 1
        });
    }

    localStorage.setItem('petalCart', JSON.stringify(cart));
    updateNavCartCount();

    const niceName = product.name || 'item';
    showToast(`Added ${niceName} to cart`, 'success', 2100);
}

// Update the navbar cart badge (called from layout script + here)
function updateNavCartCount() {
    try {
        const cart = JSON.parse(localStorage.getItem('petalCart') || '[]');
        const count = cart.reduce((sum, item) => sum + (item.qty || 1), 0);
        const badge = document.getElementById('nav-cart-count');
        if (badge) {
            if (count > 0) {
                badge.textContent = count > 99 ? '99+' : count;
                badge.style.display = 'flex';
            } else {
                badge.style.display = 'none';
            }
        }
    } catch (e) {}
}

// Helper to fetch from the flower API with correct versioning
async function fetchFromAPI(path, options = {}) {
    let url = path.startsWith('http') ? path : `${PETAL_API_BASE}${path}`;

    // Ensure the version is provided as a query param because the API uses
    // QueryStringApiVersionReader("FlowerStore-API-Version").
    // Also include it as a header (the CORS policy explicitly allows this header).
    const versionParam = `${PETAL_VERSION_HEADER}=${encodeURIComponent(PETAL_API_VERSION)}`;
    if (!url.includes(PETAL_VERSION_HEADER)) {
        url += (url.includes('?') ? '&' : '?') + versionParam;
    }

    const headers = {
        [PETAL_VERSION_HEADER]: PETAL_API_VERSION,
        ...(options.headers || {})
    };

    // Only send Content-Type when we actually have a body.
    const method = (options.method || 'GET').toUpperCase();
    if (method !== 'GET' && method !== 'HEAD' && options.body) {
        headers['Content-Type'] = 'application/json';
    }

    const res = await fetch(url, {
        ...options,
        headers
    });
    if (!res.ok) {
        const text = await res.text().catch(() => '');
        throw new Error(`API error ${res.status}: ${text || res.statusText}`);
    }
    return res.json();
}

// Build a friendly emoji for a product (based on name or category)
function getProductEmoji(product, categoryName = '') {
    const name = (product.name || '').toLowerCase();
    const cat = (categoryName || '').toLowerCase();
    if (name.includes('rose') || cat.includes('bouquet')) return '🌹';
    if (name.includes('lily') || cat.includes('box')) return '🪷';
    if (name.includes('tulip') || name.includes('crocus')) return '🌷';
    if (name.includes('orchid')) return '🌺';
    if (name.includes('daisy') || name.includes('gerbera')) return '🌼';
    if (name.includes('sunflower')) return '🌻';
    if (cat.includes('single')) return '🌸';
    if (cat.includes('wrap')) return '💐';
    return '🌿';
}
