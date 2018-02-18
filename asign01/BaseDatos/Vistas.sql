/****** Object:  View [dbo].[vae_cat_profesor]    Script Date: 02/17/2018 21:53:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE TABLE [dbo].[vae_cat_profesor]
(
	[rpe] INT NOT NULL PRIMARY KEY, 
    [titulo] NCHAR(10) NULL, 
    [nombre] NCHAR(100) NULL
)
GO
/****** Object:  View [dbo].[vae_cat_materia]    Script Date: 02/17/2018 21:53:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].vae_cat_materia
(
	[cve_materia] INT NOT NULL PRIMARY KEY, 
    [cve_area] INT NULL, 
    [nombre_l] NCHAR(100) NULL
)
GO
/****** Object:  View [dbo].[vae_cat_area]    Script Date: 02/17/2018 21:53:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE TABLE [dbo].[vae_cat_area]
(
	[cve_area] INT NOT NULL PRIMARY KEY, 
    [area] NCHAR(30) NULL 
)
GO