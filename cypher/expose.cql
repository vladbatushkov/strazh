MATCH (n)
WHERE n:Class OR n:Interface OR n:Method
UNWIND split(n.modifiers, ",") as x
WITH x, n
WHERE x in ["public", "internal"]
RETURN labels(n)[0] as name, x, count(n) as count