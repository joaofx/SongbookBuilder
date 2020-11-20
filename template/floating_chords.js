window.onload = function initFloatingChords() {
    // init "sticky" style which will keep chords at the top
    const stickyStyle = document.createElement("style")
    stickyStyle.type = "text/css"
    stickyStyle.innerHTML = ".sticky { position: fixed; top: 0; left: 0; padding: 0px 8px; width: 100%; border-bottom: 1px solid #cfd8dc;}"
    document.getElementsByTagName('head')[0].appendChild(stickyStyle);

    const chordsContainer = document.getElementsByClassName("mt-4")[0];
    // prevent lyrics being visible from behind the chords 
    chordsContainer.style.backgroundColor = "white"

    const chordsContainerTop = chordsContainer.offsetTop
    const chordsContainerHeight = chordsContainer.clientHeight

    const chords = Array.prototype.slice.call(chordsContainer.getElementsByTagName("img"))
    const maxChordSize = chords[0].clientWidth
    const minChordSize = 40

    const tab = document.getElementById("tab")

    window.onscroll = function() {
        onScrollListener()
    };

    function onScrollListener() {
        var ratio = (window.pageYOffset - chordsContainerTop) / (chordsContainerTop + chordsContainerHeight)
        ratio = Math.min(1.0, ratio)

        var size = (1.0 - ratio) * maxChordSize
        // minChordSize < size < maxChordSize
        size = Math.min(maxChordSize, Math.max(minChordSize, size));
        chords.forEach(chords => chords.style.width = size + "px")

        if (window.pageYOffset > chordsContainerTop) {
            chordsContainer.classList.add("sticky");
            //include margin to prevent tab jumping up
            tab.style.marginTop = chordsContainerHeight + "px"
        } else {
            chordsContainer.classList.remove("sticky");
            tab.style.marginTop = "0px"
        }
    }
}