MATCH (n)
WHERE n:Class OR n:Interface OR n:Method
UNWIND split(n.modifiers, ",") as x
WITH x, n
WHERE x in ["public", "internal"]
RETURN labels(n)[0] as name, x as modifier, count(n) as count

MATCH (n)
WHERE (n:Class OR n:Interface OR n:Method)
AND n.modifiers =~ ".*public.*"
RETURN labels(n)[0] as name, n.modifiers as modifier, count(n) as count