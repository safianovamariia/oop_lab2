<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template match="/Schedule">
    <html>
      <head>
        <meta charset="utf-8" />
        <title>Display class schedule</title>
        <style>
          body { font-family: Segoe UI, Arial, sans-serif; margin: 24px; }
          h1 { font-size: 22px; }
          h2 { font-size: 17px; margin-top: 28px; }
          table { border-collapse: collapse; min-width: 640px; }
          th, td { border: 1px solid #999; padding: 6px 10px; text-align: left; }
          th { background: #e8e8e8; }
        </style>
      </head>
      <body>
        <h1>Schedule of faculty display classes and department ARMs</h1>
        <p><xsl:value-of select="@faculty" />, <xsl:value-of select="@semester" /></p>
        <xsl:apply-templates select="Room" />
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Room">
    <h2>
      Room <xsl:value-of select="@number" />, building <xsl:value-of select="@building" />
      (<xsl:choose>
        <xsl:when test="@type = 'DEPARTMENT_ARM'">department ARM</xsl:when>
        <xsl:otherwise>display class</xsl:otherwise>
      </xsl:choose>, <xsl:value-of select="@department" />,
      <xsl:value-of select="@workplacesCount" /> workplaces)
    </h2>
    <table>
      <tr>
        <th>Day</th>
        <th>Time</th>
        <th>Type</th>
        <th>Details</th>
      </tr>
      <xsl:apply-templates select="Session" />
    </table>
  </xsl:template>

  <xsl:template match="Session">
    <tr>
      <td><xsl:value-of select="@day" /></td>
      <td><xsl:value-of select="@time" /></td>
      <xsl:choose>
        <xsl:when test="@subject">
          <td>Lesson</td>
          <td><xsl:value-of select="@subject" />, teacher <xsl:value-of select="@teacher" />, group <xsl:value-of select="@group" /></td>
        </xsl:when>
        <xsl:when test="@workplace">
          <td>ARM work</td>
          <td>Workplace <xsl:value-of select="@workplace" />, user <xsl:value-of select="@user" /></td>
        </xsl:when>
        <xsl:otherwise>
          <td>Free access</td>
          <td>Responsible: <xsl:value-of select="@responsible" /></td>
        </xsl:otherwise>
      </xsl:choose>
    </tr>
  </xsl:template>
</xsl:stylesheet>
