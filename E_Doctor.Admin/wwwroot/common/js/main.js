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

/**
* Recursively freezes an object and all of its nested objects.
* This ensures true immutability for complex, nested data structures.
*
* @param {object} object - The object to be deep-frozen.
* @returns {object} The deep-frozen object.
*/
function deepFreeze(object) {
    // 1. Get all the property names of the object
    const propertyNames = Object.getOwnPropertyNames(object);

    // 2. Iterate over each property
    for (const name of propertyNames) {
        const value = object[name];

        // 3. Check if the property's value is an object (and not null)
        //    'typeof null' is 'object', so we must explicitly check for null.
        if (value && typeof value === 'object') {
            // 4. If it's an object, recursively call deepFreeze on it.
            //    This is the "deep" part—it freezes the inner object first.
            deepFreeze(value);
        }
    }

    // 5. Finally, freeze the current (outer) object.
    //    By this point, all nested objects have already been frozen.
    return Object.freeze(object);
}