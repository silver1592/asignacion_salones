USE [asignacion]
GO
/****** Object:  UserDefinedFunction [asignacion].[Grupos_a_las]    Script Date: 02/18/2018 20:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fernando Alvarez Flores
-- Create date: 20/06/2017
-- Description:	Obtiene los grupos que se esten impartiendo antes de la hora dada
-- =============================================
CREATE FUNCTION [asignacion].[Grupos_a_las] 
(	
	@ini int,
	@fin int
)
RETURNS TABLE 
AS
RETURN 
(
	select * from ae_horario
	where ((lunes_ini >= @ini and lunes_ini < @fin) or (lunes_fin < @fin and lunes_fin > @ini)) or
	((martes_ini>=@ini and martes_ini<@fin) or (martes_fin<@fin and martes_fin>@ini)) or
	((miercoles_ini>=@ini and miercoles_ini<@ini) or (miercoles_ini<@fin and miercoles_fin>@ini)) or
	((jueves_ini>=@ini and jueves_ini<@ini) or (jueves_ini<@fin and jueves_fin>@ini)) or 
	((viernes_ini>=@ini and viernes_ini<@ini) or (viernes_ini<@fin and viernes_fin>@ini)) or 
	((sabado_ini>=@ini and sabado_ini<@ini) or (sabado_ini<@fin and sabado_fin>@ini))
)
GO
/****** Object:  UserDefinedFunction [asignacion].[ae_Grupos_ini]    Script Date: 02/18/2018 20:14:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fernando Alvarez Flores
-- Create date: <Create Date,,>
-- Description:	Obtiene todos los grupos que inicien a la hora dada
-- =============================================
CREATE FUNCTION [asignacion].[ae_Grupos_ini]
(	
	@hora int 
)
RETURNS TABLE 
AS
RETURN 
(
	select * 
	from ae_horario
	where (lunes_ini=@hora or martes_ini=@hora or miercoles_ini=@hora or jueves_ini=@hora or viernes_ini=@hora or sabado_ini=@hora)
)
GO
