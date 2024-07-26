function safeStringify(obj) {
  return JSON.stringify(obj, (key, value) =>
    typeof value === "bigint" ? value.toString() : value
  );
}

module.exports = safeStringify;
