async function apiFetch(url, { method = 'GET', params = null, body = null } = {}) {
    const $loader = $("#app-loader");

    try {

        $loader.addClass("active");

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
            const error = await response.text();
            alert(error);

            console.error(`HTTP ${response.status}: ${response.statusText}`);
            return null;
        }

        const contentType = response.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            return await response.json();
        } else {
            return await response.text();
        }

        return null; 
    }
    catch (error) {
        console.error('Fetch failed:', error);
        return null;
    }
    finally {
        setTimeout(() => {
            $loader.removeClass("active");
        }, 500);
    }
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

function registerEvent(root, type, callback) {
    $(root).off(type).on(type, function (event) {
        if (callback) {
            callback(event);
        }
    })
}

function ValidateInput(input, message) {

    if (IsNullUndefinedEmpty(input)) {
        alert(message);

        return false;
    }

    return true;
}

function ValidateInput(input = [], message) {

    if (input.length == 0) {
        alert(message);

        return false;
    }

    return true;
}

function convertDateTimeToLocal(utcDateString) {

    if (!utcDateString) return "";

    utcDateString = `${utcDateString}Z`;

    const localDate = new Date(utcDateString);

    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    const month = months[localDate.getMonth()];
    const day = localDate.getDate().toString().padStart(2, '0');
    const year = localDate.getFullYear();

    let hours = localDate.getHours();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // The hour '0' should be '12'
    const minutes = localDate.getMinutes().toString().padStart(2, '0');

    return `${month}-${day}-${year} ${hours}:${minutes} ${ampm}`;
}