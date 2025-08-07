async function apiFetch(url, { method = 'GET', params = null, body = null } = {}) {
    if (params) {
        const queryString = new URLSearchParams(params).toString();
        url = `${url}?${queryString}`;
    }

    const options = {
        method,
        headers: {
            'Content-Type': 'application/json'
        }
    };

    if (body) {
        options.body = JSON.stringify(body);
    }

    const response = await fetch(url, options);

    if (!response.ok) {
        console.error(`HTTP ${response.status}: ${response.statusText}`);
        return null;
    }

    const contentType = response.headers.get("content-type");
    if (contentType && contentType.includes("application/json")) {
        return await response.json();
    } else {
        return await response.text(); // fallback for plain string
    }

    return null; // For NoContent (204) responses
}

function IsNullUndefinedEmpty(value) {
    if (value === null) {
        return true;
    }

    if (typeof value === 'undefined') {
        return true;
    }

    if (value === '') {
        return true;
    }

    return false;
}

