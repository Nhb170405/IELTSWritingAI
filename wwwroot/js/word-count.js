const essayText = document.getElementById("essayText");
const wordCount = document.getElementById("wordCount");

function countWords(value) {
    return value
        .trim()
        .split(/\s+/)
        .filter(Boolean).length;
}

function updateWordCount() {
    wordCount.textContent = countWords(essayText.value);
}

if (essayText && wordCount) {
    essayText.addEventListener("input", updateWordCount);
    updateWordCount();
}
