// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const asyncContentLoadedEventName = "asyncContentLoaded"

const initializeSliders = sliderSetup();
initializeSliders();
document.addEventListener(asyncContentLoadedEventName, (e) => {
    initializeSliders();
})

initializeCollapsibleAsync();

/*
 * Slider setup, generates a function which can be called repeatedly to initialize any new sliders which have appeared in the dom
 */
function sliderSetup(){
    let initializedTables = [];

    const exp = 3.0;
    const k = 1.0;
    const valueFun = (val) => Math.pow(val, exp) + k * val;

    const valueMap = (value, min, max) => {
        let range = max - min;
        let mappedMax = valueFun(range);
        let mappedVal = valueFun(value-min);
        return Math.floor(mappedVal / mappedMax * range + min - 1);
    };

    const initializationFunction = () => {
        let sliders = document.getElementsByClassName("js-table-slider");
        Array.from(sliders).forEach((slider) => {
            let tableId = slider.getAttribute("data-tableid");
            if (initializedTables.indexOf(tableId) >= 0) return;
            initializedTables.push(tableId);

            let min = parseInt(slider.getAttribute("min"));
            let max = parseInt(slider.getAttribute("max"));

            let table = document.getElementById(tableId);
            let rows = Array.from(table.getElementsByTagName("tr"));

            let update = (value) => {
                let mappedCount = valueMap(value, min, max);
                rows.forEach((row, index) => {
                    if (index < mappedCount + 1) {
                        row.classList.remove("d-none");
                    }
                    else {
                        row.classList.add("d-none");
                    }
                });
            };

            update(parseInt(slider.value));
            slider.addEventListener("change", (e) => update(parseInt(e.target.value)));
        });
    }

    return initializationFunction;
};

/*
 * Initialized collapsible async components, by attaching a handler to relevant events and tracking previously called components
 */
function initializeCollapsibleAsync(){
    let components = document.getElementsByClassName("js-collapsible-async");
    Array.from(components).forEach((component) => {
        let contentId = component.getAttribute("data-contentid");
        let endpoint = component.getAttribute("data-endpoint");
        let content = document.getElementById(contentId);

        let called = false;
        component.addEventListener("click", (e) => {
            if (called) return;
            called = true;
            const event = new Event(asyncContentLoadedEventName)
            fetch(endpoint)
                .then((response) => {
                    let status = parseInt(response.status);
                    return (status < 200 || status > 299) ? "Service failed. Error code: " + status : response.text();
                })
                .then((responseText) => content.innerHTML = responseText)
                .then(() => document.dispatchEvent(event))
                .catch(() => content.innerHTML = "Unexpected error occured");
        });
    })
}